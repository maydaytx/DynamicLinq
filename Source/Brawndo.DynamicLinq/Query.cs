using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using Brawndo.DynamicLinq.ClauseItems;
using Brawndo.Emit;

namespace Brawndo.DynamicLinq
{
	[Serializable]
	internal class Query : IEnumerable<object>, ISerializable
	{
		private readonly DB db;
		private readonly string tableName;
		private IList<Tuple<string, ClauseItem>> selectClauseItems;
		private ClauseItem whereClause;
		private IEnumerable<object> results;
		private bool needsDuck;

		internal Query(DB db, string tableName)
		{
			this.db = db;
			this.tableName = tableName;
		}

		private Query(SerializationInfo info, StreamingContext context)
		{
			results = (IEnumerable<object>) info.GetValue("Results", typeof (IEnumerable<object>));
		}

		internal void SetSelectClauseItems(IList<Tuple<string, ClauseItem>> clauseItems)
		{
			selectClauseItems = clauseItems;

			needsDuck = clauseItems == null || clauseItems.Count != 1 || clauseItems[0].Item1 != null;

			if (!needsDuck)
				selectClauseItems[0] = new Tuple<string, ClauseItem>(string.Empty, selectClauseItems[0].Item2);
		}

		internal void AddWhereClause(ClauseItem clauseItem)
		{
			if (ReferenceEquals(whereClause, null))
				whereClause = clauseItem;
			else
				whereClause = new BinaryOperation(SimpleOperator.And, whereClause, clauseItem);
		}

		private void Execute()
		{
			IDictionary<string, Type> dataTypes = new Dictionary<string, Type>();
			IList<Tuple<string, object>[]> rows = new List<Tuple<string, object>[]>();

			using (IDbConnection connection = db.GetConnection())
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					IList<Tuple<string, object>> parameters = new List<Tuple<string, object>>();
					IDictionary<string, Type> conversions = new Dictionary<string, Type>();

					command.CommandText = BuildSQL(parameters, conversions);

					foreach (Tuple<string, object> parameter in parameters)
					{
						IDbDataParameter dataParameter = command.CreateParameter();

						dataParameter.ParameterName = parameter.Item1;
						dataParameter.Value = parameter.Item2;

						command.Parameters.Add(dataParameter);
					}

					using (IDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							Tuple<string, object>[] row = new Tuple<string, object>[reader.FieldCount];

							for (int i = 0; i < reader.FieldCount; ++i)
							{
								string name = reader.GetName(i);

								Type dataType = reader.GetFieldType(i);

								object value = reader.GetValue(i);
								value = value == DBNull.Value ? null : value;

								bool useFirstConversion = !needsDuck && conversions.Count > 0;

								if (conversions.ContainsKey(name) || useFirstConversion)
								{
									if (useFirstConversion)
										dataType = Enumerable.First(conversions.Values);
									else
										dataType = conversions[name];

									if (value != null)
										value = Convert(value, dataType);
								}

								Type currentDataType;

								if (dataTypes.TryGetValue(name, out currentDataType))
								{
									if (value == null)
									{
										if (currentDataType.IsValueType && !IsNullable(currentDataType))
											dataTypes[name] = typeof (Nullable<>).MakeGenericType(currentDataType);
										else if (dataType.IsValueType && !IsNullable(dataType))
											dataTypes[name] = typeof (Nullable<>).MakeGenericType(dataType);
									}
								}
								else
								{
									if (value == null && dataType.IsValueType && !IsNullable(dataType))
										dataType = typeof (Nullable<>).MakeGenericType(dataType);

									dataTypes.Add(name, dataType);
								}

								row[i] = new Tuple<string, object>(name, value);
							}

							rows.Add(row);
						}
					}
				}
			}

			if (needsDuck)
			{
				Type duckType = DuckRepository.GenerateDuckType(Enumerable.Select(dataTypes, t => new Tuple<string, Type>(t.Key, t.Value)));

				results = Enumerable.Select(rows, row => DuckRepository.CreateDuck(duckType, Enumerable.Select(row, column => new Tuple<string, Type, object>(column.Item1, dataTypes[column.Item1], column.Item2))));
			}
			else
			{
				results = Enumerable.Select(rows, row => row.Single().Item2);
			}
		}

		private static bool IsNullable(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
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
			else if (IsNullable(type))
			{
				Type valueType = type.GetGenericArguments()[0];

				return type.GetConstructor(new[] {valueType}).Invoke(new[] {Convert(value, valueType)});
			}
			else
			{
				return System.Convert.ChangeType(value, type);
			}
		}

		private string BuildSQL(IList<Tuple<string, object>> parameters, IDictionary<string, Type> conversions)
		{
			LinkedListStringBuilder sql = "SELECT ";
			bool notFirst = false;

			if (selectClauseItems == null || selectClauseItems.Count == 0)
			{
				sql.Append("*");
			}
			else if (!needsDuck)
			{
				ClauseItem item = CheckForConversion(selectClauseItems[0], conversions);

				sql.Append(item.BuildClause(db.Dialect, parameters));
			}
			else
			{
				foreach (var property in selectClauseItems)
				{
					if (notFirst)
						sql.Append(", ");
					else
						notFirst = true;

					ClauseItem item = CheckForConversion(property, conversions);

					sql.Append(item.BuildClause(db.Dialect, parameters));
					sql.Append(" AS [" + property.Item1 + "]");
				}
			}

			sql.Append(" FROM [" + tableName + "]");

			if (!ReferenceEquals(whereClause, null))
			{
				sql.Append(" WHERE ");
				sql.Append(whereClause.BuildClause(db.Dialect, parameters));
			}

			return sql.ToString();
		}

		private static ClauseItem CheckForConversion(Tuple<string, ClauseItem> property, IDictionary<string, Type> conversions)
		{
			ClauseItem item = property.Item2;

			if (item is ConvertOperation)
			{
				ConvertOperation convert = (ConvertOperation) item;

				conversions.Add(property.Item1, convert.Type);

				item = convert.Item;
			}

			return item;
		}

		private void EnsureExecution()
		{
			if (results == null)
				Execute();
		}

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			EnsureExecution();

			return results.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			EnsureExecution();

			return results.GetEnumerator();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			EnsureExecution();

			info.AddValue("Results", results.ToList(), typeof (IEnumerable<object>));
		}
	}
}
