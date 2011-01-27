using DynamicLinq.Collections;
using DynamicLinq.Dialects;

namespace DynamicLinq.ClauseItems
{
	internal static class ClauseItemExtensions
	{
		internal static LinkedListStringBuilder BuildAndParenthesize(this ClauseItem clauseItem, IDialect dialect, ParameterCollection parameterCollection)
		{
			LinkedListStringBuilder builder = clauseItem.BuildClause(dialect, parameterCollection);

			if (clauseItem.ShouldParenthesize)
			{
				builder.Prepend("(");
				builder.Append(")");
			}

			return builder;
		}
	}
}
