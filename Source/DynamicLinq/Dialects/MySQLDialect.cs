using DynamicLinq.Collections;

namespace DynamicLinq.Dialects
{
	public class MySQLDialect : Dialect
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
