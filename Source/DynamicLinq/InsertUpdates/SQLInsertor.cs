using System;
using System.Data;
using System.Reflection;
using DynamicLinq.ClauseItems;
using DynamicLinq.Collections;
using DynamicLinq.Dialects;

namespace DynamicLinq.InsertUpdates
{
	public class SQLInsertor : IInsertor
	{
		private readonly SQLDialect dialect;
		private readonly object[] rows;

		internal SQLInsertor(SQLDialect dialect, object[] rows)
		{
			this.dialect = dialect;
			this.rows = rows;
		}

		public void Into(Func<dynamic, object> getTableName)
		{
			NameGetter nameGetter = new NameGetter();

			string tableName = (string) getTableName(nameGetter);

			using (IDbConnection connection = dialect.GetConnection())
			{
				connection.Open();

				using (IDbCommand command = connection.CreateCommand())
				{
					LinkedListStringBuilder sql = new LinkedListStringBuilder();

					ParameterCollection parameters = new ParameterCollection(new ParameterNameProvider(dialect));

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
								values.Append(constant.BuildClause(dialect, parameters));
							}

							sql.Append(string.Format("INSERT INTO [{0}] (", tableName));
							sql.Append(columns);
							sql.Append(") VALUES (");
							sql.Append(values);
							sql.Append(");\n");
						}
					}

					foreach (Parameter parameter in parameters)
					{
						IDbDataParameter dataParameter = command.CreateParameter();

						dataParameter.ParameterName = parameter.Name;
						dataParameter.Value = parameter.Value;

						command.Parameters.Add(dataParameter);
					}

					command.CommandText = sql.ToString();

					command.ExecuteNonQuery();
				}
			}
		}
	}
}