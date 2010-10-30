using System;
using System.Collections.Generic;

namespace Brawndo.DynamicLinq.ClauseItems
{
	public class UnaryOperation : ClauseItem
	{
		private readonly UnaryOperator @operator;
		private readonly ClauseItem item;

		internal UnaryOperation(UnaryOperator @operator, ClauseItem item)
		{
			this.@operator = @operator;
			this.item = item;
		}

		internal override LinkedListStringBuilder BuildClause(IList<Tuple<string, object>> parameters)
		{
			string operation;

			switch (@operator)
			{
				case UnaryOperator.Positive:
					operation = "+";
					break;
				case UnaryOperator.Negative:
					operation = "-";
					break;
				case UnaryOperator.Not:
					operation = "NOT ";
					break;
				case UnaryOperator.Complement:
					operation = "~";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return operation + item.BuildClause(parameters);
		}
	}
}
