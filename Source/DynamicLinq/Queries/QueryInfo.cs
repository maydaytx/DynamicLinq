using System;
using System.Collections.Generic;

namespace DynamicLinq.Queries
{
	public class QueryInfo
	{
		private readonly string query;
		private readonly IEnumerable<Parameter> parameters;
		private readonly IDictionary<string, Type> selectConversions;
		private readonly bool isSingleColumnSelect;

		public string Query
		{
			get { return query; }
		}

		public IEnumerable<Parameter> Parameters
		{
			get { return parameters; }
		}

		public IDictionary<string, Type> SelectConversions
		{
			get { return selectConversions; }
		}

		public bool IsSingleColumnSelect
		{
			get { return isSingleColumnSelect; }
		}

		public QueryInfo(string query, IEnumerable<Parameter> parameters, IDictionary<string, Type> selectConversions, bool isSingleColumnSelect)
		{
			this.query = query;
			this.parameters = parameters;
			this.selectConversions = selectConversions;
			this.isSingleColumnSelect = isSingleColumnSelect;
		}
	}
}