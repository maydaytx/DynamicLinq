namespace DynamicLinq.Dialect
{
	public class PostgreSQLDialect : SQLDialect
	{
		public override void SkipTakeClause(Collections.LinkedListStringBuilder builder, int? skipCount, int? takeCount)
		{
			builder.Append(" LIMIT ");

			if (takeCount != null)
				builder.Append(takeCount.ToString());
			else
				builder.Append("ALL");

			if (skipCount != null)
				builder.Append(" OFFSET " + skipCount);
		}
	}
}
