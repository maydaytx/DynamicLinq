using System;
using System.Collections.Generic;
using System.ComponentModel;
using DynamicLinq.ClauseItems;

namespace DynamicLinq.Queries
{
	public interface IQueryBuilder
	{
		void AddWhereClause(ClauseItem clauseItem);
		void WithSelector(IEnumerable<Tuple<string, ClauseItem>> selections, SelectType selectType, IDictionary<string, Type> conversions);
		void WithJoin(ClauseItem joinClause, string innerTableName);
		void AddOrderBy(ClauseItem clauseItem, ListSortDirection sortDirection);
		void AddSkip(int count);
		void SetTake(int count);
		void SetCountSelector();
		QueryInfo Build();
	}

	public enum SelectType
	{
		All,
		Single,
		Multiple
	}
}
