using System;
using System.Collections.Generic;
using System.Linq;
using DynamicLinq.ClauseItems;

namespace DynamicLinq
{
	public static class LinqExtensions
	{
		public static IEnumerable<object> Select(this object source, Func<dynamic, object> selector)
		{
			Query query = source as Query;

			if (query == null)
				throw new ArgumentOutOfRangeException("source");

			ClauseGetter clauseGetter = new ClauseGetter();

			object obj = selector(clauseGetter);

			if (obj == clauseGetter)
				query.SetSelectClauseItems(null);
			else if (obj is ClauseItem)
				query.SetSelectClauseItems(new[] {new Tuple<string, ClauseItem>(null, (ClauseItem) obj)});
			else
				query.SetSelectClauseItems(Enumerable.Select(obj.GetType().GetProperties(), p => new Tuple<string, ClauseItem>(p.Name, (ClauseItem)p.GetValue(obj, null))).ToList());

			return query;
		}

		public static IEnumerable<object> Where(this object source, Func<dynamic, object> predicate)
		{
			Query query = source as Query;

			if (query == null)
				throw new ArgumentOutOfRangeException("source");

			ClauseGetter clauseGetter = new ClauseGetter();

			object obj = predicate(clauseGetter);

			if (obj is ClauseItem)
			    query.AddWhereClause((ClauseItem) obj);
			else
			    throw new ArgumentException("Invalid predicate");

			return query;
		}
	}
}
