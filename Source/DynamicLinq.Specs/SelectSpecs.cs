using System;
using System.Collections.Generic;
using System.Linq;
using DynamicLinq.Databases;
using Machine.Specifications;

namespace DynamicLinq
{
	public class When_selecting_a_single_column
	{
		private static DB db;
		private static IList<string> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Name1"},
				new {Id = 2, Name = "Name2"},
				new {Id = 3, Name = "Name3"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   select record.Name).Cast<string>().ToList();
		};

		It should_retrieve_the_records = () =>
		{
			results[0].ShouldEqual("Name1");
			results[1].ShouldEqual("Name2");
			results[2].ShouldEqual("Name3");
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}

	public class When_selecting_and_explicitly_casting_a_single_column
	{
		private static DB db;
		private static Exception exception;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Name1"},
				new {Id = 2, Name = "Name2"},
				new {Id = 3, Name = "Name3"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			exception = Catch.Exception(() => (from record in db.Query(x => x.Table)
											   select (int)record.Id).ToList());
		};

		It should_not_allow = () =>
			exception.ShouldNotBeNull();
	}

	public class When_selecting_and_explicitly_converting_a_single_column
	{
		private static DB db;
		private static IList<int> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Name1"},
				new {Id = 2, Name = "Name2"},
				new {Id = 3, Name = "Name3"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   select record.Id.To<int>()).Cast<int>().ToList();
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}

	public class When_selecting_and_explicitly_converting_a_single_column_that_can_be_null
	{
		private static DB db;
		private static IList<int?> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Value] INTEGER)");

			db.Insert(
				new {Id = 1},
				new {Id = 2, Value = 1},
				new {Id = 3})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   select record.Value.To<int?>()).Cast<int?>().ToList();
		};

		It should_retrieve_the_records = () =>
		{
			results[0].ShouldEqual(null);
			results[1].ShouldEqual(1);
			results[2].ShouldEqual(null);
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}

	public class When_selecting_an_expression_without_a_supplied_name
	{
		private static DB db;
		private static IList<string> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [FirstName] TEXT, [LastName] TEXT)");

			db.Insert(
				new {Id = 1, FirstName = "First1", LastName = "Last1"},
				new {Id = 2, FirstName = "First2", LastName = "Last2"},
				new {Id = 3, FirstName = "First3", LastName = "Last3"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   select "" + record.FirstName + " " + record.LastName).Cast<string>().ToList();
		};

		It should_retrieve_the_records = () =>
		{
			results[0].ShouldEqual("First1 Last1");
			results[1].ShouldEqual("First2 Last2");
			results[2].ShouldEqual("First3 Last3");
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}

	public class When_selecting_all_columns
	{
		private static DB db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Name1"},
				new {Id = 2, Name = "Name2"},
				new {Id = 3, Name = "Name3"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   select record).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Id).ShouldEqual(1L);
			((object)results[0].Name).ShouldEqual("Name1");
			((object)results[1].Id).ShouldEqual(2L);
			((object)results[1].Name).ShouldEqual("Name2");
			((object)results[2].Id).ShouldEqual(3L);
			((object)results[2].Name).ShouldEqual("Name3");
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}

	public class When_selecting_specific_columns
	{
		private static DB db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [FirstName] TEXT, [LastName] TEXT)");

			db.Insert(
				new {Id = 1, FirstName = "First1", LastName = "Last1"},
				new {Id = 2, FirstName = "First2", LastName = "Last2"},
				new {Id = 3, FirstName = "First3", LastName = "Last3"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   select new { record.Id, record.FirstName }).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Id).ShouldEqual(1L);
			((object)results[0].FirstName).ShouldEqual("First1");
			((object)results[1].Id).ShouldEqual(2L);
			((object)results[1].FirstName).ShouldEqual("First2");
			((object)results[2].Id).ShouldEqual(3L);
			((object)results[2].FirstName).ShouldEqual("First3");
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}

	public class When_selecting_specific_columns_where_one_can_be_null
	{
		private static DB db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Value] INTEGER, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Name1"},
				new {Id = 2, Value = 1, Name = "Name2"},
				new {Id = 3, Value = 3, Name = "Name3"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   select new { record.Id, record.Value, record.Name }).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Id).ShouldEqual(1L);
			((object)results[0].Value).ShouldEqual(null);
			((object)results[0].Name).ShouldEqual("Name1");
			((object)results[1].Id).ShouldEqual(2L);
			((object)results[1].Value).ShouldEqual(1L);
			((object)results[1].Name).ShouldEqual("Name2");
			((object)results[2].Id).ShouldEqual(3L);
			((object)results[2].Value).ShouldEqual(3L);
			((object)results[2].Name).ShouldEqual("Name3");
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}

	public class When_selecting_specific_columns_and_explicitly_converting_one
	{
		private static DB db;
		private static IList<dynamic> results;

		public enum Status
		{
			SomeStatus1 = 1,
			SomeStatus2 = 2,
			SomeStatus3 = 3
		}

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT, [Status] INTEGER)");

			db.Insert(
				new {Id = 1, Name = "Name1", Status = Status.SomeStatus1},
				new {Id = 2, Name = "Name2", Status = Status.SomeStatus2},
				new {Id = 3, Name = "Name3", Status = Status.SomeStatus3})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
			           select new {record.Id, Status = record.Status.To<Status>()}).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Id).ShouldEqual(1L);
			((object)results[0].Status).ShouldEqual(Status.SomeStatus1);
			((object)results[1].Id).ShouldEqual(2L);
			((object)results[1].Status).ShouldEqual(Status.SomeStatus2);
			((object)results[2].Id).ShouldEqual(3L);
			((object)results[2].Status).ShouldEqual(Status.SomeStatus3);
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}

	public class When_selecting_an_expression_with_a_supplied_name
	{
		private static DB db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [FirstName] TEXT, [LastName] TEXT)");

			db.Insert(
				new {Id = 1, FirstName = "First1", LastName = "Last1"},
				new {Id = 2, FirstName = "First2", LastName = "Last2"},
				new {Id = 3, FirstName = "First3", LastName = "Last3"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
			           select new {Name = "" + record.FirstName + " " + record.LastName}).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Name).ShouldEqual("First1 Last1");
			((object)results[1].Name).ShouldEqual("First2 Last2");
			((object)results[2].Name).ShouldEqual("First3 Last3");
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}

	public class When_selecting_a_byte_array
	{
		private static DB db;
		private static IList<dynamic> results;
		private static byte[] bytes;

		Establish context = () =>
		{
			bytes = new byte[] {0xDE, 0xAD, 0xBE, 0xEF};

			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Data] BLOB)");

			db.Insert(new {Id = 1, Data = bytes}).Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
			           select new {record.Data}).ToList();
		};

		It should_retrieve_the_records = () =>
			((byte[]) results[0].Data).SequenceEqual(bytes);

		It should_retrieve_1_record = () =>
			results.Count.ShouldEqual(1);
	}
}
