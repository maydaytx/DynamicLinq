using System;
using System.Data;

namespace DynamicLinq.Dialects
{
	public class SQLiteDialect : SQLDialect
	{
		public SQLiteDialect(Func<IDbConnection> getConnection) : base(getConnection) { }
	}
}
