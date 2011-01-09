using System.Collections;
using System.Collections.Generic;

namespace DynamicLinq.Queries
{
	public class ExtendedQuery : IEnumerable<object>
	{
		private readonly DB db;
		private readonly QueryBuilder queryBuilder;

		internal ExtendedQuery(DB db, QueryBuilder queryBuilder)
		{
			this.db = db;
			this.queryBuilder = queryBuilder;
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

			return this;
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
			return ((IEnumerable<object>)this).GetEnumerator();
		}
	}
}
