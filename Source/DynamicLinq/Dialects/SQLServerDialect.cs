using System;
using System.Data;
using DynamicLinq.Collections;

namespace DynamicLinq.Dialects
{
	public class SQLServerDialect : SQLDialect
	{
		public SQLServerDialect(Func<IDbConnection> getConnection) : base(getConnection) { }

		public override string ConcatenateOperator
		{
			get { return "+"; }
		}

		public override string DateTimeFormat
		{
			get { return "yyyy-MM-dd HH:mm:ss.fff"; }
		}

		public override void SkipTakeClause(LinkedListStringBuilder builder, int? skipCount, int? takeCount)
		{
			throw new NotSupportedException("Skip() and Take() are not supported currently for SQL Server");
		}
	}
}
