﻿using System;
using System.Data;
using System.Data.SQLite;
using Brawndo.DynamicLinq.Dialect;

namespace Brawndo.DynamicLinq
{
	internal static class SQLite
	{
		public static DB GetDB(string sql, params Tuple<string, object>[] parameters)
		{
			Func<IDbConnection> getConnection = () =>
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

			return new DB(getConnection, new SQLiteDialect());
		}
	}
}
