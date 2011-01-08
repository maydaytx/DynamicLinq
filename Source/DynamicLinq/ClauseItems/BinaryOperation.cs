using System;
using System.Collections.Generic;
using DynamicLinq.Collections;
using DynamicLinq.Dialect;

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

		internal override LinkedListStringBuilder BuildClause(SQLDialect dialect, IList<Tuple<string, object>> parameters, ParameterNameProvider nameProvider)
		{
			LinkedListStringBuilder builder = leftItem.BuildClause(dialect, parameters, nameProvider);

			builder.Append(" ");
			builder.Append(@operator.GetOperator(dialect));
			builder.Append(" ");
			builder.Append(rightItem.BuildClause(dialect, parameters, nameProvider));

			return builder;
		}
	}
}
