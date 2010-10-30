using System;
using System.Data;
using System.Data.SQLite;

namespace Brawndo.DynamicLinq
{
	internal static class SQLite
	{
		public static Func<IDbConnection> CreateInMemoryDatabase(string sql, params Tuple<string, object>[] parameters)
		{
			return () =>
			{
				SQLiteConnection connection = new SQLiteConnection("Data source=:memory:");

				connection.Open();

				using (SQLiteCommand command = connection.CreateCommand())
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
		}
	}
}
