using System;
using System.Linq;
using DynamicLinq.Collections;

namespace DynamicLinq.Queries
{
	public abstract class QueryConnection : IDisposable
	{
		private readonly QueryInfo queryInfo;

		protected bool IsDisposed { get; set; }

		protected QueryConnection(QueryInfo queryInfo)
		{
			this.queryInfo = queryInfo;
		}

		protected abstract IQueryReader Reader { get; }

		internal bool Read(out object obj)
		{
			if (IsDisposed)
			{
				obj = null;
				return false;
			}

			if (Reader.Read())
			{
				if (queryInfo.IsSingleColumnSelect)
				{
					obj = GetColumn(0).Item2;
				}
				else
				{
					DynamicBag dynamicBag = new DynamicBag();

					for (int i = 0; i < Reader.FieldCount; ++i)
					{
						Tuple<string, object> value = GetColumn(i);

						dynamicBag.SetValue(value.Item1, value.Item2);
					}

					obj = dynamicBag;
				}

				return true;
			}
			else
			{
				((IDisposable) this).Dispose();

				obj = null;
				return false;
			}
		}

		private Tuple<string, object> GetColumn(int index)
		{
			string name = Reader.GetName(index);

			object value = Reader.GetValue(index);
			value = value == DBNull.Value ? null : value;

			bool useFirstConversion = queryInfo.IsSingleColumnSelect && queryInfo.SelectConversions.Count > 0;

			if (queryInfo.SelectConversions.ContainsKey(name) || useFirstConversion)
			{
				Type dataType;

				if (useFirstConversion)
					dataType = queryInfo.SelectConversions.Values.First();
				else
					dataType = queryInfo.SelectConversions[name];

				if (value != null)
					value = Convert(value, dataType);
			}

			return new Tuple<string, object>(name, value);
		}

		private static object Convert(object value, Type type)
		{
			if (type.IsEnum)
			{
				if (value is string)
					return Enum.Parse(type, (string) value);
				else
					return Enum.ToObject(type, value);
			}
			else if (value is string)
			{
				if (type == typeof (Guid))
					return new Guid((string) value);
				else if (type == typeof (DateTime))
					return DateTime.Parse((string) value);
				else
					throw new InvalidCastException("string to " + type.FullName);
			}
			else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
			{
				Type valueType = type.GetGenericArguments()[0];

				return type.GetConstructor(new[] {valueType}).Invoke(new[] {Convert(value, valueType)});
			}
			else
			{
				return System.Convert.ChangeType(value, type);
			}
		}

		public abstract void Dispose();
	}
}