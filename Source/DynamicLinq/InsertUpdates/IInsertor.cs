using System;

namespace DynamicLinq.InsertUpdates
{
	public interface IInsertor
	{
		void Into(Func<dynamic, object> getTableName);
	}
}
