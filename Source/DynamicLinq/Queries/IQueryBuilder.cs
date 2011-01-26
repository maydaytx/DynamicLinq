using System;
using System.ComponentModel;

namespace DynamicLinq.Queries
{
	public interface IQueryBuilder
	{
		void AddWhereClause(Func<object, object> predicate, ClauseGetter clauseGetter);
		void WithSelector(Func<object, object> selector, ClauseGetter clauseGetter);
		void WithJoin(Func<object, object> outerKeySelector, Func<object, object> innerKeySelector, Func<object, object, object> resultSelector, ClauseGetter outerClauseGetter, ClauseGetter innerClauseGetter, string innerTableName);
		void AddOrderBy(Func<object, object> keySelector, ClauseGetter clauseGetter, ListSortDirection sortDirection);
		void AddSkip(int count);
		void SetTake(int count);
		void SetCountSelector();
		QueryInfo Build();
	}
}
