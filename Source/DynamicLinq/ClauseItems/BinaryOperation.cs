using DynamicLinq.Collections;
using DynamicLinq.Dialects;

namespace DynamicLinq.ClauseItems
{
	public class BinaryOperation : ClauseItem
	{
		private readonly SimpleOperator @operator;
		private readonly ClauseItem leftItem;
		private readonly ClauseItem rightItem;

		internal BinaryOperation(SimpleOperator @operator, ClauseItem leftItem, ClauseItem rightItem)
		{
			this.@operator = @operator;
			this.leftItem = leftItem;
			this.rightItem = rightItem;
		}

		public override bool ShouldParenthesize
		{
			get { return true; }
		}

		public override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			LinkedListStringBuilder builder = leftItem.BuildAndParenthesize(dialect, parameters);
			builder.Append(" ");
			builder.Append(@operator.GetOperator(dialect));
			builder.Append(" ");
			builder.Append(rightItem.BuildAndParenthesize(dialect, parameters));

			return builder;
		}

		public override string ToString()
		{
			return @operator + "(" + leftItem.ToString() + ", " + rightItem.ToString() + ")";
		}
	}
}
