using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DynamicLinq.ClauseItems;
using DynamicLinq.Collections;

namespace DynamicLinq
{
	public class Insertor
	{
		private readonly DB db;
		private readonly object[] rows;

		internal Insertor(DB db, object[] rows)
		{
			this.db = db;
			this.rows = rows;
		}

		public void Into(Func<dynamic, object> getTableName)
		{
			NameGetter nameGetter = new NameGetter();

			string tableName = (string) getTableName(nameGetter);

			using (IDbConnection connection = db.GetConnection())
			{
				connection.Open();

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
								values.Append(constant.BuildClause(db.Dialect, parameters, new ParameterNameProvider()));
							}

							sql.Append(string.Format("INSERT INTO [{0}] (", tableName));
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
	}
}