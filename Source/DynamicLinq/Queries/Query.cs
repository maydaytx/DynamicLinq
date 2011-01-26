using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DynamicLinq.Dialects;

namespace DynamicLinq.Queries
{
	public class Query : IEnumerable<object>
	{
		private readonly IDialect dialect;
		private readonly string tableName;
		private readonly ClauseGetter clauseGetter;
		private readonly IQueryBuilder queryBuilder;

		internal Query(IDialect dialect, string tableName)
		{
			this.dialect = dialect;
			this.tableName = tableName;
			clauseGetter = new ClauseGetter(tableName);
			queryBuilder = dialect.GetQueryBuilder(tableName);
		}

		public ExtendedQuery Select(Func<dynamic, object> selector)
		{
			queryBuilder.WithSelector(selector, clauseGetter);

			return new ExtendedQuery(dialect, queryBuilder);
		}

		public Query Where(Func<dynamic, object> predicate)
		{
			queryBuilder.AddWhereClause(predicate, clauseGetter);

			return this;
		}

		public ExtendedQuery Join(Query inner, Func<dynamic, object> outerKeySelector, Func<dynamic, object> innerKeySelector, Func<dynamic, dynamic, object> resultSelector)
		{
			queryBuilder.WithJoin(outerKeySelector, innerKeySelector, resultSelector, clauseGetter, inner.clauseGetter, inner.tableName);

			return new ExtendedQuery(dialect, queryBuilder);
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

		public int Count()
		{
			return (int) LongCount();
		}

		public long LongCount()
		{
			queryBuilder.SetCountSelector();

			using (IEnumerator<object> enumerator = ((IEnumerable<object>) this).GetEnumerator())
			{
				enumerator.MoveNext();

				return Convert.ToInt64(enumerator.Current);
			}
		}

		public ExtendedQuery Skip(int count)
		{
			queryBuilder.AddSkip(count);

			return new ExtendedQuery(dialect, queryBuilder);
		}

		public IEnumerable<dynamic> Take(int count)
		{
			queryBuilder.SetTake(count);

			return this;
		}

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			return new QueryEnumerator(dialect, queryBuilder.Build());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>) this).GetEnumerator();
		}
	}
}
