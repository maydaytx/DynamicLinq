using System;
using System.Collections.Generic;
using System.Linq;
using DynamicLinq.Collections;
using DynamicLinq.Dialect;

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

		internal override LinkedListStringBuilder BuildClause(SQLDialect dialect, IList<Tuple<string, object>> parameters, ParameterNameProvider nameProvider)
		{
			LinkedListStringBuilder builder = item.BuildClause(dialect, parameters, nameProvider);

			dialect.InOperator(builder, list.Select(listItem => listItem.BuildClause(dialect, parameters, nameProvider)));

			return builder;
		}
	}
}