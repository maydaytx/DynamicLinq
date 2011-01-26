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

		internal override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			LinkedListStringBuilder builder = leftItem.BuildClause(dialect, parameters);

			builder.Append(" ");
			builder.Append(@operator.GetOperator(dialect));
			builder.Append(" ");
			builder.Append(rightItem.BuildClause(dialect, parameters));

			return builder;
		}
	}
}
