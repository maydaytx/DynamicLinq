using System.Collections.Generic;
using DynamicLinq.Collections;

namespace DynamicLinq.Dialect
{
	public abstract class SQLDialect
	{
		public virtual string AddOperator
		{
			get { return "+"; }
		}

		public virtual string ConcatenateOperator
		{
			get { return "||"; }
		}

		public virtual void ConcatenateStrings(LinkedListStringBuilder builder, LinkedListStringBuilder appendee)
		{
			builder.Append(ConcatenateOperator);
			builder.Append(appendee);
		}

		public virtual string SubtractOperator
		{
			get { return "-"; }
		}

		public virtual string MultiplyOperator
		{
			get { return "*"; }
		}

		public virtual string DivideOperator
		{
			get { return "/"; }
		}

		public virtual string ModOperator
		{
			get { return "%"; }
		}

		public virtual string AndOperator
		{
			get { return "AND"; }
		}

		public virtual string EqualOperator
		{
			get { return "="; }
		}

		public virtual string NotEqualOperator
		{
			get { return "<>"; }
		}

		public virtual void CompareEqualToNull(LinkedListStringBuilder builder)
		{
			builder.Append(" IS NULL");
		}

		public virtual void CompareNotEqualToNull(LinkedListStringBuilder builder)
		{
			builder.Append(" IS NOT NULL");
		}

		public virtual string OrOperator
		{
			get { return "OR"; }
		}

		public virtual string LessThanOperator
		{
			get { return "<"; }
		}

		public virtual string GreaterThanOperator
		{
			get { return ">"; }
		}

		public virtual string LessThanOrEqualOperator
		{
			get { return "<="; }
		}

		public virtual string GreaterThanOrEqualOperator
		{
			get { return ">="; }
		}

		public virtual string LikeOperator
		{
			get { return "LIKE"; }
		}

		public virtual string PositiveOperator
		{
			get { return "+"; }
		}

		public virtual string NegativeOperator
		{
			get { return "-"; }
		}

		public virtual string NotOperator
		{
			get { return "NOT"; }
		}

		public virtual string ComplementOperator
		{
			get { return "~"; }
		}

		public virtual void InOperator(LinkedListStringBuilder builder, IEnumerable<LinkedListStringBuilder> list)
		{
			bool first = true;

			foreach (LinkedListStringBuilder item in list)
			{
				if (first)
					first = false;
				else
					builder.Append(", ");

				builder.Append(item);
			}

			builder.Append(")");
		}

		public virtual string DateTimeFormat
		{
			get { return "yyyy-MM-dd HH:mm:ss.fffffff"; }
		}

		public virtual string ParameterPrefix
		{
			get { return "@"; }
		}
	}
}
