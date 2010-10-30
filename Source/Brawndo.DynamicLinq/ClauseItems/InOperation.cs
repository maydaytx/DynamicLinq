using System;
using System.Collections.Generic;

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

		internal override LinkedListStringBuilder BuildClause(IList<Tuple<string, object>> parameters)
		{
			LinkedListStringBuilder clause = item.BuildClause(parameters) + " IN (";
			bool first = true;

			foreach (ClauseItem clauseItem in list)
			{
				if (first)
					first = false;
				else
					clause += ", ";

				clause += clauseItem.BuildClause(parameters);
			}

			return clause + ")";
		}
	}
}