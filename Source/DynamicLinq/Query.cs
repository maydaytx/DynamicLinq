using System;
using System.Collections;
using System.Collections.Generic;

namespace DynamicLinq
{
	public class Query : IEnumerable<object>
	{
		private readonly DB db;
		private readonly string tableName;
		private readonly ClauseGetter clauseGetter;
		private readonly QueryBuilder queryBuilder;
		private readonly IList<object> results;
		private QueryConnection queryConnection;

		internal Query(DB db, string tableName)
		{
			this.db = db;
			this.tableName = tableName;
			clauseGetter = new ClauseGetter(tableName);
			queryBuilder = new QueryBuilder(db, tableName);
			results = new List<object>();
		}

		public IEnumerable<dynamic> Select(Func<dynamic, object> selector)
		{
			queryBuilder.WithSelector(selector, clauseGetter);

			return this;
		}

		public Query Where(Func<dynamic, object> predicate)
		{
			queryBuilder.WithWhereClause(predicate, clauseGetter);

			return this;
		}

		public IEnumerable<dynamic> Join(Query inner, Func<dynamic, object> outerKeySelector, Func<dynamic, object> innerKeySelector, Func<dynamic, dynamic, object> resultSelector)
		{
			queryBuilder.WithJoin(outerKeySelector, innerKeySelector, resultSelector, clauseGetter, inner.clauseGetter, inner.tableName);

			return this;
		}

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			if (queryConnection == null)
				queryConnection = new QueryConnection(db, queryBuilder.Build());

			return new QueryEnumerator(results, queryConnection);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>) this).GetEnumerator();
		}
	}
}
