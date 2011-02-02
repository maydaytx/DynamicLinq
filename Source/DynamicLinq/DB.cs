using System;
using System.Data;
using DynamicLinq.Dialects;
using DynamicLinq.InsertUpdates;
using DynamicLinq.Queries;

namespace DynamicLinq
{
	public sealed class DB
	{
		private readonly SQLDialect dialect;
		private readonly Func<IDbConnection> getConnection;

		public DB(SQLDialect dialect, Func<IDbConnection> getConnection)
		{
			this.dialect = dialect;
			this.getConnection = getConnection;
		}

		public Query Query(Func<dynamic, object> getTableName)
		{
			NameGetter nameGetter = new NameGetter();

			string tableName = (string) getTableName(nameGetter);

			Func<QueryInfo, QueryConnection> getQueryConnection = queryInfo => new SQLQueryConnection(getConnection, queryInfo);

			return new Query(getQueryConnection, dialect, tableName, new SQLQueryBuilder(dialect, tableName));
		}

		public SQLInsertor Insert(params object[] rows)
		{
			return new SQLInsertor(getConnection, dialect, rows);
		}

		public SQLUpdator Update(Func<dynamic, object> getTableName)
		{
			NameGetter nameGetter = new NameGetter();

			string tableName = (string) getTableName(nameGetter);

			return new SQLUpdator(getConnection, dialect, tableName);
		}
	}
}
