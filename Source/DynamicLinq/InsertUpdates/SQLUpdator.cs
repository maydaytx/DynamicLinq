using DynamicLinq.Dialects;

namespace DynamicLinq.InsertUpdates
{
	public class SQLUpdator : IUpdator
	{
		private readonly SQLDialect dialect;
		private readonly string tableName;

		internal SQLUpdator(SQLDialect dialect, string tableName)
		{
			this.dialect = dialect;
			this.tableName = tableName;
		}

		public IUpdateExecutor Set(object row)
		{
			return new SQLUpdateExecutor(dialect, tableName, row);
		}
	}
}