using System;
using DynamicLinq.Collections;

namespace DynamicLinq.Dialect
{
	public class SQLServerDialect : SQLDialect
	{
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
