using System;
using System.Collections.Generic;
using Brawndo.DynamicLinq.Dialect;

namespace Brawndo.DynamicLinq.ClauseItems
{
	public class Column : ClauseItem
	{
		private readonly string tableName;
		private readonly string name;

		internal string Name
		{
			get { return name; }
		}

		internal Column(string tableName, string name)
		{
			this.tableName = tableName;
			this.name = name;
		}

		internal override LinkedListStringBuilder BuildClause(SQLDialect dialect, IList<Tuple<string, object>> parameters)
		{
			return string.Format("[{0}].[{1}]", tableName, name);
		}
	}
}