using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace DynamicLinq.Queries
{
	public class Query : IEnumerable<object>
	{
		private readonly DB db;
		private readonly string tableName;
		private readonly ClauseGetter clauseGetter;
		private readonly QueryBuilder queryBuilder;

		internal Query(DB db, string tableName)
		{
			this.db = db;
			this.tableName = tableName;
			clauseGetter = new ClauseGetter(tableName);
			queryBuilder = new QueryBuilder(db, tableName);
		}

		public ExtendedQuery Select(Func<dynamic, object> selector)
		{
			queryBuilder.WithSelector(selector, clauseGetter);

			return new ExtendedQuery(db, queryBuilder);
		}

		public Query Where(Func<dynamic, object> predicate)
		{
			queryBuilder.AddWhereClause(predicate, clauseGetter);

			return this;
		}

		public ExtendedQuery Join(Query inner, Func<dynamic, object> outerKeySelector, Func<dynamic, object> innerKeySelector, Func<dynamic, dynamic, object> resultSelector)
		{
			queryBuilder.WithJoin(outerKeySelector, innerKeySelector, resultSelector, clauseGetter, inner.clauseGetter, inner.tableName);

			return new ExtendedQuery(db, queryBuilder);
		}

		public Query OrderBy(Func<dynamic, object> keySelector)
		{
			queryBuilder.AddOrderBy(keySelector, clauseGetter, ListSortDirection.Ascending);

			return this;
		}

		public Query OrderByDescending(Func<dynamic, object> keySelector)
		{
			queryBuilder.AddOrderBy(keySelector, clauseGetter, ListSortDirection.Descending);

			return this;
		}

		//public int Count()
		//{
			
		//}

		//public long LongCount()
		//{

		//}

		public ExtendedQuery Skip(int count)
		{
			queryBuilder.AddSkip(count);

			return new ExtendedQuery(db, queryBuilder);
		}

		public IEnumerable<dynamic> Take(int count)
		{
			queryBuilder.SetTake(count);

			return this;
		}

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			return new QueryEnumerator(db, queryBuilder.Build());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>) this).GetEnumerator();
		}
	}
}
