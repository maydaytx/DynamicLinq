using System;
using Microsoft.WindowsAzure;

namespace DynamicLinq.AzureTables
{
	public class AzureTablesInsertor
	{
		private readonly CloudStorageAccount storageAccount;
		private readonly AzureTablesObject[] rows;

		public AzureTablesInsertor(CloudStorageAccount storageAccount, AzureTablesObject[] rows)
		{
			this.storageAccount = storageAccount;
			this.rows = rows;
		}

		public void Into(Func<dynamic, object> getTableName)
		{
			NameGetter nameGetter = new NameGetter();

			string tableName = (string) getTableName(nameGetter);

			AzureTablesInsertUpdateExecutor.Insert(storageAccount, tableName, rows);
		}
	}
}