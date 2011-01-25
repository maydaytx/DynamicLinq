using System;
using System.Data;
using System.Data.SQLite;
using DynamicLinq.Dialects;

namespace DynamicLinq.Databases
{
	internal class SQLite
	{
		internal static DB GetDB(string sql)
		{
			UndisposableMemoryDatabase connection = new UndisposableMemoryDatabase();

			using (IDbCommand command = connection.CreateCommand())
			{
				command.CommandText = sql;

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

				//var info = new System.IO.FileInfo("Test.s3db");
				//if (info.Exists) info.Delete();
				//connection = new SQLiteConnection("Data source=Test.s3db");

				connection.Open();
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
