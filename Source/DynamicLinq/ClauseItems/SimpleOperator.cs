using System;
using System.Linq;
using System.Reflection;
using DynamicLinq.Dialects;

namespace DynamicLinq.ClauseItems
{
	internal class SimpleOperator
	{
		internal static readonly SimpleOperator Positive = new SimpleOperator(dialect => dialect.PositiveOperator);
		internal static readonly SimpleOperator Negative = new SimpleOperator(dialect => dialect.NegativeOperator);
		internal static readonly SimpleOperator Not = new SimpleOperator(dialect => dialect.NotOperator);
		internal static readonly SimpleOperator Complement = new SimpleOperator(dialect => dialect.ComplementOperator);

		internal static readonly SimpleOperator Add = new SimpleOperator(dialect => dialect.AddOperator);
		internal static readonly SimpleOperator Subtract = new SimpleOperator(dialect => dialect.SubtractOperator);
		internal static readonly SimpleOperator Multiply = new SimpleOperator(dialect => dialect.MultiplyOperator);
		internal static readonly SimpleOperator Divide = new SimpleOperator(dialect => dialect.DivideOperator);
		internal static readonly SimpleOperator Mod = new SimpleOperator(dialect => dialect.ModOperator);
		internal static readonly SimpleOperator And = new SimpleOperator(dialect => dialect.AndOperator);
		internal static readonly SimpleOperator Or = new SimpleOperator(dialect => dialect.OrOperator);
		internal static readonly SimpleOperator Equal = new SimpleOperator(dialect => dialect.EqualOperator);
		internal static readonly SimpleOperator NotEqual = new SimpleOperator(dialect => dialect.NotEqualOperator);
		internal static readonly SimpleOperator LessThan = new SimpleOperator(dialect => dialect.LessThanOperator);
		internal static readonly SimpleOperator GreaterThan = new SimpleOperator(dialect => dialect.GreaterThanOperator);
		internal static readonly SimpleOperator LessThanOrEqual = new SimpleOperator(dialect => dialect.LessThanOrEqualOperator);
		internal static readonly SimpleOperator GreaterThanOrEqual = new SimpleOperator(dialect => dialect.GreaterThanOrEqualOperator);
		internal static readonly SimpleOperator Like = new SimpleOperator(dialect => dialect.LikeOperator);

		private readonly Func<IDialect, string> getOperator;

		internal Func<IDialect, string> GetOperator
		{
			get { return getOperator; }
		}

		private SimpleOperator(Func<IDialect, string> getOperator)
		{
			this.getOperator = getOperator;
		}

		public override string ToString()
		{
			return GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Static).First(x => x.GetValue(null) == this).Name;
		}
	}
}
