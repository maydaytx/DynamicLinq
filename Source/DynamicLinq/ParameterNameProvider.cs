using DynamicLinq.Dialects;

namespace DynamicLinq
{
	internal class ParameterNameProvider
	{
		private readonly SQLDialect dialect;
		private int count;

		internal ParameterNameProvider(SQLDialect dialect)
		{
			this.dialect = dialect;
		}

		internal string GetParameterName()
		{
			return dialect.ParameterPrefix + "p" + count++;
		}
	}
}