using System.Collections.Generic;
using System.Linq;
using DynamicLinq.Collections;
using DynamicLinq.Dialects;

namespace DynamicLinq.ClauseItems
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

		internal override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			LinkedListStringBuilder builder = item.BuildClause(dialect, parameters);

			dialect.InOperator(builder, list.Select(listItem => listItem.BuildClause(dialect, parameters)));

			return builder;
		}
	}
}