using DynamicLinq.Collections;

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

		internal override LinkedListStringBuilder BuildClause(Dialect dialect, ParameterCollection parameters)
		{
			LinkedListStringBuilder builder = item.BuildClause(dialect, parameters);

			if (compareEqualToNull)
				dialect.CompareEqualToNull(builder);
			else
				dialect.CompareNotEqualToNull(builder);

			return builder;
		}
	}
}
