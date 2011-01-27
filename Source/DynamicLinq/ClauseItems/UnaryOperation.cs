using DynamicLinq.Collections;
using DynamicLinq.Dialects;

namespace DynamicLinq.ClauseItems
{
	public class UnaryOperation : ClauseItem
	{
		private readonly SimpleOperator @operator;
		private readonly ClauseItem item;

		internal UnaryOperation(SimpleOperator @operator, ClauseItem item)
		{
			this.@operator = @operator;
			this.item = item;
		}

		public override bool ShouldParenthesize
		{
			get { return false; }
		}

		public override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			LinkedListStringBuilder builder = @operator.GetOperator(dialect);
			builder.Append(item.BuildAndParenthesize(dialect, parameters));

			return builder;
		}

		public override string ToString()
		{
			return @operator + "(" + item.ToString() + ")";
		}
	}
}
