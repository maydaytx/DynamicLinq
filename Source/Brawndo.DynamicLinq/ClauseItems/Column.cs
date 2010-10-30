using System;
using System.Collections.Generic;

namespace Brawndo.DynamicLinq.ClauseItems
{
	public class Column : ClauseItem
	{
		private readonly string name;

		internal string Name
		{
			get { return name; }
		}

		internal Column(string name)
		{
			this.name = name;
		}

		internal override LinkedListStringBuilder BuildClause(IList<Tuple<string, object>> parameters)
		{
			return new LinkedListStringBuilder("[" + name + "]");
		}
	}
}