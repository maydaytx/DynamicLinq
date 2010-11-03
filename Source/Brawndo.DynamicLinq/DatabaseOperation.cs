namespace Brawndo.DynamicLinq
{
	internal class DatabaseOperation
	{
		private readonly DB db;
		private readonly string tableName;

		internal DB DB
		{
			get { return db; }
		}

		internal string TableName
		{
			get { return tableName; }
		}

		internal DatabaseOperation(DB db, string tableName)
		{
			this.db = db;
			this.tableName = tableName;
		}
	}
}
