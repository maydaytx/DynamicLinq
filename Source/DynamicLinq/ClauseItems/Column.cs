using System;
using System.Collections.Generic;
using DynamicLinq.Collections;
using DynamicLinq.Dialect;

namespace DynamicLinq.ClauseItems
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

		internal override LinkedListStringBuilder BuildClause(SQLDialect dialect, IList<Tuple<string, object>> parameters, ParameterNameProvider nameProvider)
		{
			return string.Format("[{0}].[{1}]", tableName, name);
		}
	}
}