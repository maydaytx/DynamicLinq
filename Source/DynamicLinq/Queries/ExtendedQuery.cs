using System;
using System.Collections;
using System.Collections.Generic;
using DynamicLinq.Dialects;

namespace DynamicLinq.Queries
{
	public class ExtendedQuery : IEnumerable<object>
	{
		private readonly Func<QueryInfo, QueryConnection> getQueryConnection;
		private readonly IDialect dialect;
		private readonly IQueryBuilder queryBuilder;

		internal ExtendedQuery(Func<QueryInfo, QueryConnection> getQueryConnection, IDialect dialect, IQueryBuilder queryBuilder)
		{
			this.getQueryConnection = getQueryConnection;
			this.dialect = dialect;
			this.queryBuilder = queryBuilder;
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

			return this;
		}

		public IEnumerable<dynamic> Take(int count)
		{
			queryBuilder.SetTake(count);

			return this;
		}

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			return new QueryEnumerator(getQueryConnection, dialect, queryBuilder.Build());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>)this).GetEnumerator();
		}
	}
}
