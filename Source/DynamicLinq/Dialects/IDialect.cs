using System.Collections.Generic;
using DynamicLinq.Collections;

namespace DynamicLinq.Dialects
{
	public interface IDialect
	{
		string AddOperator { get; }

		void ConcatenateStrings(LinkedListStringBuilder builder, LinkedListStringBuilder appendee);

		string SubtractOperator { get; }

		string MultiplyOperator { get; }

		string DivideOperator { get; }

		string ModOperator { get; }

		string AndOperator { get; }

		string EqualOperator { get; }

		string NotEqualOperator { get; }

		string EqualNull { get; }

		string NotEqualNull { get; }

		string OrOperator { get; }

		string LessThanOperator { get; }

		string GreaterThanOperator { get; }

		string LessThanOrEqualOperator { get; }

		string GreaterThanOrEqualOperator { get; }

		string LikeOperator { get; }

		string PositiveOperator { get; }

		string NegativeOperator { get; }

		string NotOperator { get; }

		string ComplementOperator { get; }

		void InOperator(LinkedListStringBuilder builder, IEnumerable<LinkedListStringBuilder> list);

		string Column(string tableName, string columnName);

		string Constant(object value, ParameterCollection parameters);
	}
}
