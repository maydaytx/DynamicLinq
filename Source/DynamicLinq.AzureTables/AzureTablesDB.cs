using System;
using DynamicLinq.Queries;
using Microsoft.WindowsAzure;

namespace DynamicLinq.AzureTables
{
	public sealed class AzureTablesDB
	{
		private readonly CloudStorageAccount storageAccount;

		public AzureTablesDB(CloudStorageAccount storageAccount)
		{
			this.storageAccount = storageAccount;
		}

		public Query Query(Func<dynamic, object> getTableName)
		{
			NameGetter nameGetter = new NameGetter();

			string tableName = (string) getTableName(nameGetter);

			Func<QueryInfo, QueryConnection> getQueryConnection = queryInfo => new AzureTablesQueryConnection(storageAccount, queryInfo);

			return new Query(getQueryConnection, new AzureTablesDialect(), tableName, new AzureTablesQueryBuilder(tableName));
		}

		public AzureTablesInsertor Insert(params AzureTablesObject[] rows)
		{
			return new AzureTablesInsertor(storageAccount, rows);
		}

		public AzureTablesUpdator Update(Func<dynamic, object> getTableName)
		{
			NameGetter nameGetter = new NameGetter();

			string tableName = (string) getTableName(nameGetter);

			return new AzureTablesUpdator(storageAccount, tableName);
		}
	}
}
