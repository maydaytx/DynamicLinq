using DynamicLinq.Collections;
using DynamicLinq.Dialects;

namespace DynamicLinq.ClauseItems
{
	public class NullComparisonOperation : ClauseItem
	{
		private readonly bool compareEqualToNull;
		private readonly ClauseItem item;

		internal NullComparisonOperation(bool compareEqualToNull, ClauseItem item)
		{
			this.compareEqualToNull = compareEqualToNull;
			this.item = item;
		}

		internal override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			LinkedListStringBuilder builder = item.BuildClause(dialect, parameters);

			if (compareEqualToNull)
				builder.Append(dialect.CompareEqualToNull);
			else
				builder.Append(dialect.CompareNotEqualToNull);

			return builder;
		}
	}
}
