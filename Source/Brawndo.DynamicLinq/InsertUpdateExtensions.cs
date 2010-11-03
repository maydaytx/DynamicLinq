using System;
using System.Data;
using System.Reflection;

namespace Brawndo.DynamicLinq
{
	public static class InsertUpdateExtensions
	{
//		public static Update Update(this object source, object row)
//		{
//			DatabaseOperation databaseOperation = source.GetDatabaseOperation();
//
//
//		}

		public static void Insert(this object source, params object[] rows)
		{
			DatabaseOperation databaseOperation = source.GetDatabaseOperation();

			using (IDbConnection connection = databaseOperation.DB.GetConnection())
			{
				foreach (object row in rows)
				{
					PropertyInfo[] properties = row.GetType().GetProperties();

					if (properties.Length > 0)
					{
						using (IDbCommand command = connection.CreateCommand())
						{
							LinkedListStringBuilder columns = new LinkedListStringBuilder();
							LinkedListStringBuilder values = new LinkedListStringBuilder();

							for (int i = 0; i < properties.Length; ++i)
							{
								IDbDataParameter dataParameter = command.CreateParameter();

								string parameterName = databaseOperation.DB.Dialect.ParameterPrefix + "p" + i;

								dataParameter.ParameterName = parameterName;
								dataParameter.Value = properties[i].GetValue(row, null);

								command.Parameters.Add(dataParameter);

								if (i > 0)
								{
									columns.Append(", ");
									values.Append(", ");
								}

								columns.Append(string.Format("[{0}]", properties[i].Name));
								values.Append(parameterName);
							}

							LinkedListStringBuilder commandText = new LinkedListStringBuilder(string.Format("INSERT INTO [{0}] (", databaseOperation.TableName));
							commandText.Append(columns);
							commandText.Append(") VALUES (");
							commandText.Append(values);
							commandText.Append(")");

							command.CommandText = commandText.ToString();

							command.ExecuteNonQuery();
						}
					}
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
		public void Where(Func<dynamic, object> predicate)
		{
			
		}
	}
}
