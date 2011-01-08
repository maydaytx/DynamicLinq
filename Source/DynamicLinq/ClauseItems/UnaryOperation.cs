using System;
using System.Collections.Generic;
using DynamicLinq.Collections;
using DynamicLinq.Dialect;

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

		internal override LinkedListStringBuilder BuildClause(SQLDialect dialect, IList<Tuple<string, object>> parameters, ParameterNameProvider nameProvider)
		{
			LinkedListStringBuilder builder = item.BuildClause(dialect, parameters, nameProvider);

			builder.Prepend(@operator.GetOperator(dialect));

			return builder;
		}
	}
}
