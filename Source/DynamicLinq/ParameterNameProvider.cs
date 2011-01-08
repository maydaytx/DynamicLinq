namespace DynamicLinq
{
	internal class ParameterNameProvider
	{
		private int count;

		internal string GetParameterName()
		{
			return "p" + count++;
		}
	}
}