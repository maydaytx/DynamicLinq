using System;
using System.Data;
using DynamicLinq.Queries;

namespace DynamicLinq
{
	public class DB
	{
		private readonly Func<IDbConnection> getConnection;
		private readonly Dialect dialect;

		internal Func<IDbConnection> GetConnection
		{
			get { return getConnection; }
		}

		internal Dialect Dialect
		{
			get { return dialect; }
		}

		public DB(Func<IDbConnection> getConnection, Dialect dialect)
		{
			this.getConnection = getConnection;
			this.dialect = dialect;
		}

		public Query Query(Func<dynamic, object> getTableName)
		{
			NameGetter nameGetter = new NameGetter();

			string tableName = (string) getTableName(nameGetter);

			return new Query(this, tableName);
		}

		public Insertor Insert(params object[] rows)
		{
			return new Insertor(this, rows);
		}

		public Updator Update(Func<dynamic, object> getTableName)
		{
			NameGetter nameGetter = new NameGetter();

			string tableName = (string)getTableName(nameGetter);

			return new Updator(this, tableName);
		}
	}
}
