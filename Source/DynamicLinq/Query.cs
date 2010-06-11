using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DynamicLinq.ClauseItems;

namespace DynamicLinq
{
	internal class Query : IEnumerable<object>
	{
		private readonly DB db;
		private readonly string tableName;
		private IList<Tuple<string, ClauseItem>> selectClauseItems;
		private ClauseItem whereClause;
		private IEnumerable<object> results;

		internal Query(DB db, string tableName)
		{
			this.db = db;
			this.tableName = tableName;
		}

		internal void SetSelectClauseItems(IList<Tuple<string, ClauseItem>> selectClauseItems)
		{
			this.selectClauseItems = selectClauseItems;
		}

		internal void AddWhereClause(ClauseItem whereClause)
		{
			if (ReferenceEquals(this.whereClause, null))
				this.whereClause = whereClause;
			else
				this.whereClause = new BinaryOperation(BinaryOperator.And, this.whereClause, whereClause);
		}

		private bool NeedsDuck
		{
			get { return selectClauseItems == null || selectClauseItems.Count != 1 || selectClauseItems[0].Item1 != null; }
		}

		private void Execute()
		{
			IDictionary<string, Type> dataTypes = new Dictionary<string, Type>();
			IList<Tuple<string, object>[]> rows = new List<Tuple<string, object>[]>();

			using (IDbConnection connection = db.GetConnection())
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					IList<Tuple<string, object>> parameters = new List<Tuple<string, object>>(); ;

					command.CommandText = BuildSQL(parameters);

					foreach (Tuple<string, object> parameter in parameters)
					{
						IDbDataParameter dataParameter = command.CreateParameter();

						dataParameter.ParameterName = parameter.Item1;
						dataParameter.Value = parameter.Item2;

						command.Parameters.Add(dataParameter);
					}

					using (IDataReader reader = command.ExecuteReader())
					{
						bool first = true;

						while (reader.Read())
						{
							Tuple<string, object>[] row = new Tuple<string, object>[reader.FieldCount];

							for (int i = 0; i < reader.FieldCount; ++i)
							{
								string name = reader.GetName(i);

								Type dataType = reader.GetFieldType(i);

								object value = reader.GetValue(i);
								value = value == DBNull.Value ? null : value;

								if (first)
									dataTypes.Add(name, dataType);
								else if (value == null && dataType.IsValueType)
									dataTypes[name] = typeof (Nullable<>).MakeGenericType(dataType);

								row[i] = new Tuple<string, object>(name, value);
							}

							rows.Add(row);

							first = false;
						}
					}
				}
			}

			if (NeedsDuck)
			{
				Type duckType = DuckRepository.GenerateDuckType(Enumerable.Select(dataTypes, t => new Tuple<string, Type>(t.Key, t.Value)));

				results = Enumerable.Select(rows, row => DuckRepository.CreateDuck(duckType, Enumerable.Select(row, column => new Tuple<string, Type, object>(column.Item1, dataTypes[column.Item1], column.Item2))));
			}
			else
			{
				results = Enumerable.Select(rows, row => row.Single().Item2);
			}
		}

		private string BuildSQL(IList<Tuple<string, object>> parameters)
		{
			AwesomeStringBuilder sql = new AwesomeStringBuilder("SELECT ");
			bool notFirst = false;

			if (selectClauseItems == null || selectClauseItems.Count == 0)
			{
				sql += "*";
			}
			else if (!NeedsDuck)
			{
				sql += selectClauseItems[0].Item2.BuildClause(parameters);
			}
			else
			{
				foreach (var property in selectClauseItems)
				{
					if (notFirst)
						sql += ", ";
					else
						notFirst = true;

					sql += property.Item2.BuildClause(parameters) + " AS [" + property.Item1 + "]";
				}
			}

			sql += " FROM [" + tableName + "]";

			if (!ReferenceEquals(whereClause, null))
				sql += " WHERE " + whereClause.BuildClause(parameters);

			return sql.ToString();
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
	}
}
