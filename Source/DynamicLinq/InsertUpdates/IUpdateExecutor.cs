using System;

namespace DynamicLinq.InsertUpdates
{
	public interface IUpdateExecutor
	{
		IUpdateExecutor Where(Func<dynamic, object> predicate);

		void Execute();
	}
}
