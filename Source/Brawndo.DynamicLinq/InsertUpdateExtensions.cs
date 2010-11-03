using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Brawndo.DynamicLinq.ClauseItems;

namespace Brawndo.DynamicLinq
{
	public static class InsertUpdateExtensions
	{
		public static Update Update(this object source, object row)
		{
			DatabaseOperation databaseOperation = source.GetDatabaseOperation();

			return new Update(databaseOperation.DB, databaseOperation.TableName, row);
		}

		public static void Insert(this object source, params object[] rows)
		{
			DatabaseOperation databaseOperation = source.GetDatabaseOperation();

			using (IDbConnection connection = databaseOperation.DB.GetConnection())
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					LinkedListStringBuilder sql = new LinkedListStringBuilder();

					IList<Tuple<string, object>> parameters = new List<Tuple<string, object>>();

					foreach (object row in rows)
					{
						PropertyInfo[] properties = row.GetType().GetProperties();

						if (properties.Length > 0)
						{
							LinkedListStringBuilder columns = new LinkedListStringBuilder();
							LinkedListStringBuilder values = new LinkedListStringBuilder();

							for (int i = 0; i < properties.Length; ++i)
							{
								Constant constant = new Constant(properties[i].GetValue(row, null));

								if (i > 0)
								{
									columns.Append(", ");
									values.Append(", ");
								}

								columns.Append(string.Format("[{0}]", properties[i].Name));
								values.Append(constant.BuildClause(databaseOperation.DB.Dialect, parameters));
							}

							sql.Append(string.Format("INSERT INTO [{0}] (", databaseOperation.TableName));
							sql.Append(columns);
							sql.Append(") VALUES (");
							sql.Append(values);
							sql.Append(");\n");
						}
					}

					foreach (Tuple<string, object> parameter in parameters)
					{
						IDbDataParameter dataParameter = command.CreateParameter();

						dataParameter.ParameterName = parameter.Item1;
						dataParameter.Value = parameter.Item2;

						command.Parameters.Add(dataParameter);
					}

					command.CommandText = sql.ToString();

					command.ExecuteNonQuery();
				}
			}
		}

		private static DatabaseOperation GetDatabaseOperation(this object source)
		{
			DatabaseOperation databaseOperation;

			if (source is DatabaseOperation)
				databaseOperation = (DatabaseOperation)source;
			else
				throw new ArgumentOutOfRangeException("source");

			return databaseOperation;
		}
	}

	public class Update
	{
		private readonly DB db;
		private readonly string tableName;
		private readonly object row;
		private ClauseItem whereClause;

		internal Update(DB db, string tableName, object row)
		{
			this.db = db;
			this.tableName = tableName;
			this.row = row;
		}

		public Update Where(Func<dynamic, object> predicate)
		{
			ClauseGetter clauseGetter = new ClauseGetter(tableName);

			object obj = predicate(clauseGetter);

			if (obj is ClauseItem)
				whereClause = (ClauseItem)obj;
			else
				throw new ArgumentException("Invalid predicate");

			return this;
		}

		public void Execute()
		{
			using (IDbConnection connection = db.GetConnection())
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					PropertyInfo[] properties = row.GetType().GetProperties();

					if (properties.Length > 0)
					{
						LinkedListStringBuilder sql = new LinkedListStringBuilder(string.Format("UPDATE [{0}] SET ", tableName));

						IList<Tuple<string, object>> parameters = new List<Tuple<string, object>>();

						for (int i = 0; i < properties.Length; ++i)
						{
							Constant constant = new Constant(properties[i].GetValue(row, null));

							if (i > 0)
								sql.Append(", ");

							sql.Append(string.Format("[{0}] = {1}", properties[i].Name, constant.BuildClause(db.Dialect, parameters)));
						}

						if (whereClause != null)
						{
							sql.Append(" WHERE ");
							sql.Append(whereClause.BuildClause(db.Dialect, parameters));
						}

						foreach (Tuple<string, object> parameter in parameters)
						{
							IDbDataParameter dataParameter = command.CreateParameter();

							dataParameter.ParameterName = parameter.Item1;
							dataParameter.Value = parameter.Item2;

							command.Parameters.Add(dataParameter);
						}

						command.CommandText = sql.ToString();

						command.ExecuteNonQuery();
					}
				}
			}
		}
	}
}
