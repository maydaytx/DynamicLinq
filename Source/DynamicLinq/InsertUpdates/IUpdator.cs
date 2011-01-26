namespace DynamicLinq.InsertUpdates
{
	public interface IUpdator
	{
		IUpdateExecutor Set(object row);
	}
}
