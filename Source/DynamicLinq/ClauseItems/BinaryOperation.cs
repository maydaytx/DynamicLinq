using System;
using System.Collections.Generic;

namespace DynamicLinq.ClauseItems
{
	public class BinaryOperation : ClauseItem
	{
		private readonly BinaryOperator @operator;
		private readonly ClauseItem leftItem;
		private readonly ClauseItem rightItem;

		internal BinaryOperation(BinaryOperator @operator, ClauseItem leftItem, ClauseItem rightItem)
		{
			this.@operator = @operator;
			this.leftItem = leftItem;
			this.rightItem = rightItem;
		}

		internal override LinkedListStringBuilder BuildClause(IList<Tuple<string, object>> parameters)
		{
			string operatorString;

			switch (@operator)
			{
				case BinaryOperator.Add:
					if (EvaluatesToString(leftItem))
						operatorString = "||";
					else
						operatorString = "+";
					break;
				case BinaryOperator.Subtract:
					operatorString = "-";
					break;
				case BinaryOperator.Multiply:
					operatorString = "*";
					break;
				case BinaryOperator.Divide:
					operatorString = "/";
					break;
				case BinaryOperator.Mod:
					operatorString = "%";
					break;
				case BinaryOperator.And:
					operatorString = "AND";
					break;
				case BinaryOperator.Or:
					operatorString = "OR";
					break;
				case BinaryOperator.Equal:
					if (ReferenceEquals(leftItem, null) && ReferenceEquals(rightItem, null))
						return new LinkedListStringBuilder("(TRUE)");
					else if (ReferenceEquals(leftItem, null))
						return "(" + rightItem.BuildClause(parameters) + "IS NULL)";
					else if (ReferenceEquals(rightItem, null))
						return "(" + leftItem.BuildClause(parameters) + "IS NULL)";
					else
						operatorString = "=";
					break;
				case BinaryOperator.NotEqual:
					if (ReferenceEquals(leftItem, null) && ReferenceEquals(rightItem, null))
						return new LinkedListStringBuilder("(FALSE)");
					else if (ReferenceEquals(leftItem, null))
						return "(" + rightItem.BuildClause(parameters) + "IS NOT NULL)";
					else if (ReferenceEquals(rightItem, null))
						return "(" + leftItem.BuildClause(parameters) + "IS NOT NULL)";
					else
						operatorString = "<>";
					break;
				case BinaryOperator.LessThan:
					operatorString = "<";
					break;
				case BinaryOperator.GreaterThan:
					operatorString = ">";
					break;
				case BinaryOperator.LessThanOrEqual:
					operatorString = "<=";
					break;
				case BinaryOperator.GreaterThanOrEqual:
					operatorString = ">=";
					break;
				case BinaryOperator.Like:
					operatorString = "LIKE";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return "(" + leftItem.BuildClause(parameters) + " " + operatorString + " " + rightItem.BuildClause(parameters) + ")";
		}

		private static bool EvaluatesToString(ClauseItem clauseItem)
		{
			if (clauseItem is Constant && ((Constant)clauseItem).Object is string)
				return true;

			BinaryOperation binaryOperation = clauseItem as BinaryOperation;

			if (!ReferenceEquals(binaryOperation, null) && binaryOperation.@operator == BinaryOperator.Add)
				return EvaluatesToString(binaryOperation.leftItem);
			else
				return false;
		}
	}
}
