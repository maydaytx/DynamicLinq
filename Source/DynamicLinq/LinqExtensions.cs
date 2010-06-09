using System;
using System.Collections.Generic;

namespace DynamicLinq
{
	public static class LinqExtensions
	{
		public static IEnumerable<object> Select(this object source, Func<dynamic, object> map)
		{
			Query query = source as Query;

			if (query == null)
				throw new ArgumentOutOfRangeException("source");

			return query.Execute();
		}
	}
}
