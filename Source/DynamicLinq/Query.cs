using System;
using System.Collections;
using System.Collections.Generic;

namespace DynamicLinq
{
	public class Query : IEnumerable<object>
	{
		private readonly DB db;
		private readonly ClauseGetter clauseGetter;
		private readonly QueryBuilder queryBuilder;
		private readonly IList<object> results;
		private QueryConnection queryConnection;

		internal Query(DB db, string tableName)
		{
			this.db = db;
			clauseGetter = new ClauseGetter(tableName);
			queryBuilder = new QueryBuilder(db, tableName);
			results = new List<object>();
		}

		public IEnumerable<object> Select(Func<dynamic, object> selector)
		{
			queryBuilder.WithSelector(selector, clauseGetter);

			return this;
		}

		public Query Where(Func<dynamic, object> predicate)
		{
			queryBuilder.WithWhereClause(predicate, clauseGetter);

			return this;
		}

		public IEnumerable<object> Join(Query inner, Func<dynamic, object> outerKeySelector, Func<dynamic, object> innerKeySelector, Func<dynamic, dynamic, object> resultSelector)
		{
			//ClauseItem outerKey = outerKeySelector(clauseGetter) as ClauseItem;
			//ClauseItem innerKey = innerKeySelector(inner.clauseGetter) as ClauseItem;

			//if (outerKey == null || innerKey == null)
			//    throw new ArgumentException("Invalid key selector");

			//queryBuilder.WithJoin(inner.tableName, new BinaryOperation(SimpleOperator.Equal, outerKey, innerKey));

			//object obj = resultSelector(clauseGetter, inner.clauseGetter);

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
