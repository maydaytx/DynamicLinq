namespace DynamicLinq
{
	public class Parameter
	{
		public string Name { get; private set; }
		public object Value { get; private set; }

		public Parameter(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}
}