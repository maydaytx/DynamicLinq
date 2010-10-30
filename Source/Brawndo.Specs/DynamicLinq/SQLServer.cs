using System;
using System.Data;
using System.Data.SqlClient;
using Brawndo.DynamicLinq.Dialect;

namespace Brawndo.DynamicLinq
{
	internal static class SQLServer
	{
		public static DB GetDB(string sql, params Tuple<string, object>[] parameters)
		{
			Func<IDbConnection> getConnection = () =>
			{
				SqlConnection connection = new SqlConnection("server=localhost\\sqlexpress;database=Test;Integrated Security=true;");

				connection.Open();

				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = "DROP TABLE [Table]";

					try
					{
						command.ExecuteNonQuery();
					}
					catch
					{
					}
				}

				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = sql;

					foreach (Tuple<string, object> parameter in parameters)
					{
						IDbDataParameter dataParameter = command.CreateParameter();

						dataParameter.ParameterName = parameter.Item1;
						dataParameter.Value = parameter.Item2;

						command.Parameters.Add(dataParameter);
					}

					command.ExecuteNonQuery();
				}

				return connection;
			};

			return new DB(getConnection, new SQLServerDialect());
		}
	}
}
