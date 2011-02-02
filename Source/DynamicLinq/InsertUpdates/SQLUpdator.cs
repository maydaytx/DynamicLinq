using System;
using System.Data;
using DynamicLinq.Dialects;

namespace DynamicLinq.InsertUpdates
{
	public class SQLUpdator
	{
		private readonly Func<IDbConnection> getConnection;
		private readonly SQLDialect dialect;
		private readonly string tableName;

		internal SQLUpdator(Func<IDbConnection> getConnection, SQLDialect dialect, string tableName)
		{
			this.getConnection = getConnection;
			this.dialect = dialect;
			this.tableName = tableName;
		}

		public SQLUpdateExecutor Set(object row)
		{
			return new SQLUpdateExecutor(getConnection, dialect, tableName, row);
		}
	}
}