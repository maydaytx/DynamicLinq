using System;
using System.Collections.Generic;
using DynamicLinq.Collections;
using DynamicLinq.Dialect;

namespace DynamicLinq.ClauseItems
{
	public class ConcatenationOperation : ClauseItem
	{
		private readonly ClauseItem leftItem;
		private readonly ClauseItem rightItem;

		internal ConcatenationOperation(ClauseItem leftItem, ClauseItem rightItem)
		{
			this.leftItem = leftItem;
			this.rightItem = rightItem;
		}

		internal override LinkedListStringBuilder BuildClause(SQLDialect dialect, IList<Tuple<string, object>> parameters, ParameterNameProvider nameProvider)
		{
			LinkedListStringBuilder builder = leftItem.BuildClause(dialect, parameters, nameProvider);

			dialect.ConcatenateStrings(builder, rightItem.BuildClause(dialect, parameters, nameProvider));

			return builder;
		}
	}
}
