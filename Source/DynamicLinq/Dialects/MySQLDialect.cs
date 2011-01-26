using System;
using System.Data;
using DynamicLinq.Collections;

namespace DynamicLinq.Dialects
{
	public class MySQLDialect : SQLDialect
	{
		public MySQLDialect(Func<IDbConnection> getConnection) : base(getConnection) { }

		public override void ConcatenateStrings(LinkedListStringBuilder builder, LinkedListStringBuilder appendee)
		{
			builder.Prepend("CONCAT(");
			builder.Append(", ");
			builder.Append(appendee);
			builder.Append(")");
		}
	}
}
