using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DynamicLinq.ClauseItems;
using DynamicLinq.Collections;

namespace DynamicLinq
{
	public class UpdateExecutor
	{
		private readonly DB db;
		private readonly string tableName;
		private readonly object row;
		private ClauseItem whereClause;

		internal UpdateExecutor(DB db, string tableName, object row)
		{
			this.db = db;
			this.tableName = tableName;
			this.row = row;
		}

		public UpdateExecutor Where(Func<dynamic, object> predicate)
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
						ParameterNameProvider nameProvider = new ParameterNameProvider();

						for (int i = 0; i < properties.Length; ++i)
						{
							Constant constant = new Constant(properties[i].GetValue(row, null));

							if (i > 0)
								sql.Append(", ");

							sql.Append(string.Format("[{0}] = {1}", properties[i].Name, constant.BuildClause(db.Dialect, parameters, nameProvider)));
						}

						if (whereClause != null)
						{
							sql.Append(" WHERE ");
							sql.Append(whereClause.BuildClause(db.Dialect, parameters, nameProvider));
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