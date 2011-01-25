using System;
using System.Collections.Generic;

namespace DynamicLinq.Queries
{
	internal class QueryInfo
	{
		private readonly string sql;
		private readonly IEnumerable<Parameter> parameters;
		private readonly IDictionary<string, Type> selectConversions;
		private readonly bool isSingleColumnSelect;

		internal string SQL
		{
			get { return sql; }
		}

		internal IEnumerable<Parameter> Parameters
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

		internal QueryInfo(string sql, IEnumerable<Parameter> parameters, IDictionary<string, Type> selectConversions, bool isSingleColumnSelect)
		{
			this.sql = sql;
			this.parameters = parameters;
			this.selectConversions = selectConversions;
			this.isSingleColumnSelect = isSingleColumnSelect;
		}
	}
}