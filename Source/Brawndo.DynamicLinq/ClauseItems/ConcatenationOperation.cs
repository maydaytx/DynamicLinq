using System;
using System.Collections.Generic;
using Brawndo.DynamicLinq.Dialect;

namespace Brawndo.DynamicLinq.ClauseItems
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

		internal override LinkedListStringBuilder BuildClause(SQLDialect dialect, IList<Tuple<string, object>> parameters)
		{
			LinkedListStringBuilder builder = leftItem.BuildClause(dialect, parameters);

			dialect.ConcatenateStrings(builder, rightItem.BuildClause(dialect, parameters));

			return builder;
		}
	}
}
