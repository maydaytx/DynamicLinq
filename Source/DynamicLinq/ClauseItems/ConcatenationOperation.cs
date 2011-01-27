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

		public override bool ShouldParenthesize
		{
			get { return false; }
		}

		public override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			LinkedListStringBuilder builder = leftItem.BuildAndParenthesize(dialect, parameters);

			dialect.ConcatenateStrings(builder, rightItem.BuildAndParenthesize(dialect, parameters));

			return builder;
		}

		public override string ToString()
		{
			return "Concatenate(" + leftItem.ToString() + ", " + rightItem.ToString() + ")";
		}
	}
}
