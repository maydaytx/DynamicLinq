using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using DynamicLinq.AzureTables;
using DynamicLinq.Databases;
using Machine.Specifications;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace DynamicLinq
{
	public class When_skipping_rows
	{
		private static DB db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Name1"},
				new {Id = 2, Name = "Name2"},
				new {Id = 3, Name = "Name3"},
				new {Id = 4, Name = "Name4"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   select record).Skip(2).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Id).ShouldEqual(3L);
			((object)results[0].Name).ShouldEqual("Name3");
			((object)results[1].Id).ShouldEqual(4L);
			((object)results[1].Name).ShouldEqual("Name4");
		};

		It should_retrieve_2_records = () =>
			results.Count.ShouldEqual(2);
	}

	public class When_taking_rows
	{
		private static DB db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Name1"},
				new {Id = 2, Name = "Name2"},
				new {Id = 3, Name = "Name3"},
				new {Id = 4, Name = "Name4"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   select record).Take(2).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Id).ShouldEqual(1L);
			((object)results[0].Name).ShouldEqual("Name1");
			((object)results[1].Id).ShouldEqual(2L);
			((object)results[1].Name).ShouldEqual("Name2");
		};

		It should_retrieve_2_records = () =>
			results.Count.ShouldEqual(2);
	}

	public class When_skipping_and_taking_rows
	{
		private static DB db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Name1"},
				new {Id = 2, Name = "Name2"},
				new {Id = 3, Name = "Name3"},
				new {Id = 4, Name = "Name4"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   select record).Skip(1).Take(2).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Id).ShouldEqual(2L);
			((object)results[0].Name).ShouldEqual("Name2");
			((object)results[1].Id).ShouldEqual(3L);
			((object)results[1].Name).ShouldEqual("Name3");
		};

		It should_retrieve_2_records = () =>
			results.Count.ShouldEqual(2);
	}

	public class When
	{
		private static AzureTablesDB db;
		private static IList<dynamic> results;

		public class TestTable : TableServiceEntity
		{
			public string WTF { get; set; }
			public int ASDF { get; set; }
		}

		Establish context = () =>
		{
			CloudTableClient client = new CloudTableClient(CloudStorageAccount.DevelopmentStorageAccount.TableEndpoint.AbsoluteUri, CloudStorageAccount.DevelopmentStorageAccount.Credentials);

			//client.DeleteTableIfExist("TestTable");
			//client.CreateTable("TestTable");

			//var context = client.GetDataServiceContext();

			//context.AddObject("TestTable", new TestTable { PartitionKey = "1", RowKey = "1", WTF = "Bool", ASDF = 4 });
			//context.AddObject("TestTable", new TestTable { PartitionKey = "1", RowKey = "2", WTF = "Bool", ASDF = 1 });
			//context.AddObject("TestTable", new TestTable { PartitionKey = "1", RowKey = "3", WTF = null, ASDF = 7 });

			//context.SaveChanges(SaveChangesOptions.Batch);

			//var tests = client.GetDataServiceContext().CreateQuery<TestTable>("TestTable").Select(x => x).ToList();

			db = new AzureTablesDB(CloudStorageAccount.DevelopmentStorageAccount);

			db.Update(x => x.TestTable)
				.With(new AzureTablesObject("1", "1", new {WTF = "LOOB", ASDF = 4})
					//,new AzureTablesObject("1", "2", new {WTF = "Bool", ASDF = 1})
					//,new AzureTablesObject("1", "3", new {WTF = default(string), ASDF = 7})
					);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.TestTable)
					   //where record.WTF == "Bool"// && record.ASDF == 1
					   //where record.PartitionKey == "1" && record.RowKey == "2"
					   select record).ToList();
		};

		It should_retrieve_the_records = () =>
		{
		};
	}

	public class When2
	{
		private static AzureTablesDB db;
		private static IList<dynamic> results;

		private static string Reverse(IEnumerable<char> str)
		{
			return new string(str.Reverse().ToArray());
		}

		Establish context = () =>
		{
			//db = new AzureTablesDB(new CloudStorageAccount(CloudStorageAccount.DevelopmentStorageAccount.Credentials, null, null, new Uri("http://127.0.0.1:10457/devstoreaccount1")));
			db = new AzureTablesDB(CloudStorageAccount.DevelopmentStorageAccount);

			results = db.Query(x => x.Foo).ToList();

			db.Update(x => x.Foo).With(new AzureTablesObject("asdf", "12", new {Bar = Reverse(results[0].Bar), ASDF = results[0].ASDF.AddYears(1)}));
		};

		Because of = () =>
		{
		};

		It should_retrieve_the_records = () =>
		{
		};
	}

	public class When3
	{
		private static AzureTablesDB db;

		Establish context = () =>
		{
			CloudTableClient client = new CloudTableClient(CloudStorageAccount.DevelopmentStorageAccount.TableEndpoint.AbsoluteUri, CloudStorageAccount.DevelopmentStorageAccount.Credentials);

			client.DeleteTableIfExist("Foo");
			client.CreateTable("Foo");

			//db = new AzureTablesDB(new CloudStorageAccount(CloudStorageAccount.DevelopmentStorageAccount.Credentials, null, null, new Uri("http://127.0.0.1:10457/devstoreaccount1")));
			db = new AzureTablesDB(CloudStorageAccount.DevelopmentStorageAccount);

			db.Insert(new AzureTablesObject("asdf", "12", new {Bar = "Bool", ASDF = DateTime.Now})).Into(x => x.Foo);
		};

		Because of = () =>
		{
		};

		It should_retrieve_the_records = () =>
		{
		};
	}
}
