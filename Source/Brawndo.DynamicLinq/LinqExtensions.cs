using System;
using System.Collections.Generic;
using System.Linq;
using Brawndo.DynamicLinq.ClauseItems;

namespace Brawndo.DynamicLinq
{
	public static class LinqExtensions
	{
		public static IEnumerable<object> Select(this object source, Func<dynamic, object> selector)
		{
            Query query = source.GetQuery();

			object obj = selector(query.ClauseGetter);

            if (obj == query.ClauseGetter)
				query.SetSelectClauseItems(null);
			else if (obj is ClauseItem)
				query.SetSelectClauseItems(new[] {new Tuple<string, ClauseItem>(null, (ClauseItem) obj)});
			else
				query.SetSelectClauseItems(Enumerable.Select(obj.GetType().GetProperties(), p => new Tuple<string, ClauseItem>(p.Name, (ClauseItem)p.GetValue(obj, null))).ToList());

			return query;
		}

		public static IEnumerable<object> Where(this object source, Func<dynamic, object> predicate)
		{
            Query query = source.GetQuery();

            object obj = predicate(query.ClauseGetter);

			if (obj is ClauseItem)
			    query.AddWhereClause((ClauseItem) obj);
			else
			    throw new ArgumentException("Invalid predicate");

			return query;
		}

        public static IEnumerable<object> Join(this object outer, object inner, Func<dynamic, object> outerKeySelector, Func<dynamic, object> innerKeySelector, Func<dynamic, dynamic, object> resultSelector)
        {
            Query outerQuery = outer.GetQuery();
            Query innerQuery = inner.GetQuery();

            object outerKey = outerKeySelector(outerQuery.ClauseGetter);
            object innerKey = innerKeySelector(innerQuery.ClauseGetter);
            object results = resultSelector(outerQuery.ClauseGetter, innerQuery.ClauseGetter);

            return null;
        }

        private static Query GetQuery(this object source)
        {
            Query query;

            if (source is Query)
            {
            	query = (Query) source;
            }
			else if (source is DatabaseOperation)
			{
				DatabaseOperation databaseOperation = (DatabaseOperation) source;

				query = new Query(databaseOperation.DB, databaseOperation.TableName);
			}
			else
			{
				throw new ArgumentOutOfRangeException("source");
			}

            return query;
        }
	}
}
