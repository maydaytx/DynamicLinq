using System;
using System.Collections;
using System.Collections.Generic;

namespace DynamicLinq.Queries
{
	internal class QueryEnumerator : IEnumerator<object>
	{
		private readonly IList<object> results;
		private readonly QueryConnection queryConnection;
		private int currentPos;

		internal QueryEnumerator(IList<object> results, QueryConnection queryConnection)
		{
			this.results = results;
			this.queryConnection = queryConnection;
			currentPos = -1;
		}

		void IDisposable.Dispose() { }

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