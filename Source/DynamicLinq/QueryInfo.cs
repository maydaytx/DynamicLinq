using System;
using System.Collections.Generic;

namespace DynamicLinq
{
	internal class QueryInfo
	{
		private readonly string sql;
		private readonly IEnumerable<Tuple<string, object>> parameters;
		private readonly IDictionary<string, Type> selectConversions;
		private readonly bool isSingleColumnSelect;

		internal string SQL
		{
			get { return sql; }
		}

		internal IEnumerable<Tuple<string, object>> Parameters
		{
			get { return parameters; }
		}

		internal IDictionary<string, Type> SelectConversions
		{
			get { return selectConversions; }
		}

		internal bool IsSingleColumnSelect
		{
			get { return isSingleColumnSelect; }
		}

		internal QueryInfo(string sql, IEnumerable<Tuple<string, object>> parameters, IDictionary<string, Type> selectConversions, bool isSingleColumnSelect)
		{
			this.sql = sql;
			this.parameters = parameters;
			this.selectConversions = selectConversions;
			this.isSingleColumnSelect = isSingleColumnSelect;
		}
	}
}