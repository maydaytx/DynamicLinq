using System;
using System.Collections;
using System.Collections.Generic;
using DynamicLinq.Dialects;

namespace DynamicLinq.Queries
{
	internal class QueryEnumerator : IEnumerator<object>
	{
		private readonly IList<object> results;
		private readonly QueryConnection queryConnection;
		private int currentPos;

		internal QueryEnumerator(IDialect dialect, QueryInfo queryInfo)
		{
			results = new List<object>();
			queryConnection = dialect.GetConnection(queryInfo);
			currentPos = -1;
		}

		void IDisposable.Dispose()
		{
			((IDisposable) queryConnection).Dispose();
		}

		bool IEnumerator.MoveNext()
		{
			if (currentPos + 1 < results.Count)
			{
				++currentPos;
				return true;
			}
			else
			{
				object obj;

				if (queryConnection.Read(out obj))
				{
					++currentPos;
					results.Add(obj);
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		void IEnumerator.Reset()
		{
			currentPos = -1;
		}

		object IEnumerator<object>.Current
		{
			get { return results[currentPos]; }
		}

		object IEnumerator.Current
		{
			get { return ((IEnumerator<object>) this).Current; }
		}
	}
}