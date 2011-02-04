using System;
using System.Collections.Generic;
using DynamicLinq.Collections;
using DynamicLinq.Dialects;

namespace DynamicLinq.AzureTables
{
	internal class AzureTablesDialect : IDialect
	{
		public string AddOperator
		{
			get { return "add"; }
		}

		public void ConcatenateStrings(LinkedListStringBuilder builder, LinkedListStringBuilder appendee)
		{
			builder.Prepend("concat(");
			builder.Append(",");
			builder.Append(appendee);
			builder.Append(")");
		}

		public string SubtractOperator
		{
			get { return "sub"; }
		}

		public string MultiplyOperator
		{
			get { return "mul"; }
		}

		public string DivideOperator
		{
			get { return "div"; }
		}

		public string ModOperator
		{
			get { return "mod"; }
		}

		public string AndOperator
		{
			get { return "and"; }
		}

		public string EqualOperator
		{
			get { return "eq"; }
		}

		public string NotEqualOperator
		{
			get { return "ne"; }
		}

		public string EqualNull
		{
			get { return "eq null"; }
		}

		public string NotEqualNull
		{
			get { return "ne null"; }
		}

		public string OrOperator
		{
			get { return "or"; }
		}

		public string LessThanOperator
		{
			get { return "lt"; }
		}

		public string GreaterThanOperator
		{
			get { return "gt"; }
		}

		public string LessThanOrEqualOperator
		{
			get { return "le"; }
		}

		public string GreaterThanOrEqualOperator
		{
			get { return "ge"; }
		}

		public string LikeOperator
		{
			get { throw new NotImplementedException("TODO!!!"); }
		}

		public string PositiveOperator
		{
			get { return string.Empty; }
		}

		public string NegativeOperator
		{
			get { return " -"; }
		}

		public string NotOperator
		{
			get { return "not"; }
		}

		public string ComplementOperator
		{
			get { return "not"; }
		}

		public void InOperator(LinkedListStringBuilder builder, IEnumerable<LinkedListStringBuilder> list)
		{
			throw new NotSupportedException("IN operator not supported currently for Azure Tables");
		}

		public string Column(string tableName, string columnName)
		{
			return columnName;
		}

		public string Constant(object value, ParameterCollection parameters)
		{
			return AzureConvert.KeyPrimitiveToString(value);
		}
	}
}
