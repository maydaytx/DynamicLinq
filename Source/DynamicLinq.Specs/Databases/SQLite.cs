using System;
using System.Data;
using System.Data.SQLite;
using Brawndo.DynamicLinq.Dialect;

namespace Brawndo.DynamicLinq
{
	internal class SQLite
	{
		internal static DB GetDB(string sql, params Tuple<string, object>[] parameters)
		{
			UndisposableMemoryDatabase connection = new UndisposableMemoryDatabase();

			connection.Open();

			using (IDbCommand command = connection.CreateCommand())
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

			Func<IDbConnection> getConnection = () => connection;

			return new DB(getConnection, new SQLiteDialect());
		}

		private class UndisposableMemoryDatabase : IDbConnection
		{
			private readonly IDbConnection connection;

			public UndisposableMemoryDatabase()
			{
				connection = new SQLiteConnection("Data source=:memory:");
			}

			public void Dispose() { }

			public IDbTransaction BeginTransaction()
			{
				return connection.BeginTransaction();
			}

			public IDbTransaction BeginTransaction(IsolationLevel il)
			{
				return connection.BeginTransaction(il);
			}

			public void Close()
			{
				connection.Close();
			}

			public void ChangeDatabase(string databaseName)
			{
				connection.ChangeDatabase(databaseName);
			}

			public IDbCommand CreateCommand()
			{
				return connection.CreateCommand();
			}

			public void Open()
			{
				connection.Open();
			}

			public string ConnectionString
			{
				get { return connection.ConnectionString; }
				set { connection.ConnectionString = value; }
			}

			public int ConnectionTimeout
			{
				get { return connection.ConnectionTimeout; }
			}

			public string Database
			{
				get { return connection.Database; }
			}

			public ConnectionState State
			{
				get { return connection.State; }
			}
		}
	}
}
