namespace DynamicLinq.Queries
{
	public interface IQueryReader
	{
		bool Read();
		int FieldCount { get; }
		string GetName(int index);
		object GetValue(int index);
	}
}
