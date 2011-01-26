using DynamicLinq.Collections;
using DynamicLinq.Dialects;

namespace DynamicLinq.ClauseItems
{
	public class ConcatenationOperation : ClauseItem
	{
		private readonly ClauseItem leftItem;
		private readonly ClauseItem rightItem;

		internal ConcatenationOperation(ClauseItem leftItem, ClauseItem rightItem)
		{
			this.leftItem = leftItem;
			this.rightItem = rightItem;
		}

		internal override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			LinkedListStringBuilder builder = leftItem.BuildClause(dialect, parameters);

			dialect.ConcatenateStrings(builder, rightItem.BuildClause(dialect, parameters));

			return builder;
		}
	}
}
