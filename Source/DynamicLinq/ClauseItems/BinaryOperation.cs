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

		internal override AwesomeStringBuilder BuildClause(IList<Tuple<string, object>> parameters)
		{
			AwesomeStringBuilder clause = new AwesomeStringBuilder("(");
			
			clause += leftItem.BuildClause(parameters) + " ";

			switch (@operator)
			{
					case BinaryOperator.Add:
						if (EvaluatesToString(leftItem))
							clause += "||";
						else
							clause += "+";
						break;
					case BinaryOperator.Subtract:
						clause += "-";
						break;
					case BinaryOperator.Multiply:
						clause += "*";
						break;
					case BinaryOperator.Divide:
						clause += "/";
						break;
					case BinaryOperator.Mod:
						clause += "%";
						break;
					case BinaryOperator.And:
						clause += "AND";
						break;
					case BinaryOperator.Or:
						clause += "OR";
						break;
					case BinaryOperator.Equal:
						if (rightItem is Constant && ((Constant) rightItem).Object == null)
							clause += "IS";
						else
							clause += "=";
						break;
					case BinaryOperator.NotEqual:
						if (rightItem is Constant && ((Constant) rightItem).Object == null)
							clause += "IS NOT";
						else
							clause += "<>";
						break;
					case BinaryOperator.LessThan:
						clause += "<";
						break;
					case BinaryOperator.GreaterThan:
						clause += ">";
						break;
					case BinaryOperator.LessThanOrEqual:
						clause += "<=";
						break;
					case BinaryOperator.GreaterThanOrEqual:
						clause += ">=";
						break;
					case BinaryOperator.Like:
						clause += "LIKE";
						break;
					default:
						throw new ArgumentOutOfRangeException();
			}

			return clause + " " + rightItem.BuildClause(parameters) + ")";
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
