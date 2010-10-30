namespace Brawndo.DynamicLinq.Dialect
{
	public class SQLServerDialect : SQLDialect
	{
		public override string ConcatenateOperator
		{
			get { return "+"; }
		}

		public override string DateTimeFormat
		{
			get { return "yyyy-MM-dd HH:mm:ss.fff"; }
		}
	}
}
