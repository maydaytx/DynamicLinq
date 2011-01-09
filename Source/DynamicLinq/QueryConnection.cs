using System;
using System.Data;
using System.Linq;
using DynamicLinq.Collections;

namespace DynamicLinq
{
	internal class QueryConnection : IDisposable
	{
		private readonly QueryInfo queryInfo;
		private IDbConnection connection;
		private IDbCommand command;
		private IDataReader reader;
		private bool isDisposed;

		internal QueryConnection(DB db, QueryInfo queryInfo)
		{
			this.queryInfo = queryInfo;

			connection = db.GetConnection();
			connection.Open();

			command = connection.CreateCommand();

			command.CommandText = queryInfo.SQL;

			foreach (Tuple<string, object> parameter in queryInfo.Parameters)
			{
				IDbDataParameter dataParameter = command.CreateParameter();

				dataParameter.ParameterName = parameter.Item1;
				dataParameter.Value = parameter.Item2;

				command.Parameters.Add(dataParameter);
			}

			reader = command.ExecuteReader();
		}

		internal bool Read(out object obj)
		{
			if (isDisposed)
			{
				obj = null;
				return false;
			}
			if (reader.Read())
			{
				if (queryInfo.IsSingleColumnSelect)
				{
					obj = GetColumn(0).Item2;
				}
				else
				{
					DynamicBag dynamicBag = new DynamicBag();

					for (int i = 0; i < reader.FieldCount; ++i)
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
			string name = reader.GetName(index);

			object value = reader.GetValue(index);
			value = value == DBNull.Value ? null : value;

			bool useFirstConversion = queryInfo.IsSingleColumnSelect && queryInfo.SelectConversions.Count > 0;

			if (queryInfo.SelectConversions.ContainsKey(name) || useFirstConversion)
			{
				Type dataType;

				if (useFirstConversion)
					dataType = Enumerable.First(queryInfo.SelectConversions.Values);
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

		void IDisposable.Dispose()
		{
			if (reader != null)
			{
				reader.Dispose();
				reader = null;
			}

			if (command != null)
			{
				command.Dispose();
				command = null;
			}

			if (connection != null)
			{
				connection.Dispose();
				connection = null;
			}

			isDisposed = true;
		}
	}
}