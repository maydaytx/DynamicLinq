using System.Reflection;
using DynamicLinq.Collections;

namespace DynamicLinq.AzureTables
{
	public class AzureTablesObject : DynamicBag
	{
		internal string ETag { get; private set; }

		public string PartitionKey
		{
			get { return (string) GetValue("PartitionKey"); }
			private set { SetValue("PartitionKey", value); }
		}

		public string RowKey
		{
			get { return (string) GetValue("RowKey"); }
			private set { SetValue("RowKey", value); }
		}

		internal AzureTablesObject(string eTag)
		{
			ETag = eTag;
		}

		public AzureTablesObject(string partitionKey, string rowKey, object values)
		{
			PartitionKey = partitionKey;
			RowKey = rowKey;

			foreach (PropertyInfo property in values.GetType().GetProperties())
				SetValue(property.Name, property.GetValue(values, null));
		}
	}
}