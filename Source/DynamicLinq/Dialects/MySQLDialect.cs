using DynamicLinq.Collections;

namespace DynamicLinq.Dialects
{
	public class MySQLDialect : SQLDialect
	{
		public override void ConcatenateStrings(LinkedListStringBuilder builder, LinkedListStringBuilder appendee)
		{
			builder.Prepend("CONCAT(");
			builder.Append(", ");
			builder.Append(appendee);
			builder.Append(")");
		}
	}
}
