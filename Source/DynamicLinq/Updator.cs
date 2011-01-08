namespace DynamicLinq
{
	public class Updator
	{
		private readonly DB db;
		private readonly string tableName;

		internal Updator(DB db, string tableName)
		{
			this.db = db;
			this.tableName = tableName;
		}

		public UpdateExecutor Set(object row)
		{
			return new UpdateExecutor(db, tableName, row);
		}
	}
}