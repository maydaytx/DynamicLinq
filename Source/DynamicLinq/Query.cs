using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DynamicLinq
{
	internal class Query : IEnumerable<object>
	{
		private string id;
		private readonly DB db;
		private readonly string tableName;

		private IEnumerable<object> results;

		internal Query(DB db, string tableName)
		{
			this.db = db;
			this.tableName = tableName;
		}

		internal void EnsureId(string id)
		{
			if (this.id == null)
				this.id = id;
		}

		private string GetId()
		{
			if (id == null)
				id = Guid.NewGuid().ToString();

			return id;
		}

		internal IEnumerable<object> Execute()
		{
			IDictionary<string, Type> dataTypes = new Dictionary<string, Type>();
			IList<Tuple<string, object>[]> rows = new List<Tuple<string, object>[]>();

			using (IDbConnection connection = db.GetConnection())
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM [" + tableName + "];";

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

			results = Enumerable.Select(rows, row => DuckRepository.GenerateDuck(GetId(), Enumerable.Select(row, column => new Tuple<string, Type, object>(column.Item1, dataTypes[column.Item1], column.Item2))));

			return results;
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
