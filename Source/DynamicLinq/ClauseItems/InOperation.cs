using System;
using System.Collections.Generic;
using System.Linq;
using Brawndo.DynamicLinq.Dialect;

namespace Brawndo.DynamicLinq.ClauseItems
{
	public class InOperation : ClauseItem
	{
		private readonly ClauseItem item;
		private readonly IEnumerable<ClauseItem> list;

		internal InOperation(ClauseItem item, IEnumerable<ClauseItem> list)
		{
			this.item = item;
			this.list = list;
		}

		internal override LinkedListStringBuilder BuildClause(SQLDialect dialect, IList<Tuple<string, object>> parameters)
		{
			LinkedListStringBuilder builder = item.BuildClause(dialect, parameters);

			dialect.InOperator(builder, Enumerable.Select(list, listItem => listItem.BuildClause(dialect, parameters)));

			return builder;
		}
	}
}