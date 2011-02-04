using Microsoft.WindowsAzure;

namespace DynamicLinq.AzureTables
{
	public class AzureTablesUpdator
	{
		private readonly CloudStorageAccount storageAccount;
		private readonly string tableName;

		internal AzureTablesUpdator(CloudStorageAccount storageAccount, string tableName)
		{
			this.storageAccount = storageAccount;
			this.tableName = tableName;
		}

		public void With(params AzureTablesObject[] rows)
		{
			AzureTablesInsertUpdateExecutor.Update(storageAccount, tableName, rows);
		}
	}
}
