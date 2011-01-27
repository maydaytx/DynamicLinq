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

		public override bool ShouldParenthesize
		{
			get { return true; }
		}

		public override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			LinkedListStringBuilder builder = item.BuildAndParenthesize(dialect, parameters);
			builder.Append(" ");

			if (compareEqualToNull)
				builder.Append(dialect.EqualNull);
			else
				builder.Append(dialect.NotEqualNull);

			return builder;
		}

		public override string ToString()
		{
			return (compareEqualToNull ? "IsNull(" : "IsNotNull(") + item.ToString() + ")";
		}
	}
}
