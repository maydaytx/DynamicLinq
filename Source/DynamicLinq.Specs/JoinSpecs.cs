using System.Collections.Generic;
using System.Linq;
using DynamicLinq.Databases;
using Machine.Specifications;

namespace DynamicLinq
{
	public class When_joining_two_tables
	{
		private static DB db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table1] ([Id] INTEGER PRIMARY KEY, [Name] TEXT, [Table2Id] INTEGER);

CREATE TABLE [Table2] ([Id] INTEGER PRIMARY KEY, [Name] TEXT);"
				);

			db.Insert(
				new {Id = 1, Name = "Name11", Table2Id = 1},
				new {Id = 2, Name = "Name12", Table2Id = 2},
				new {Id = 3, Name = "Name13", Table2Id = 1},
				new {Id = 4, Name = "Name14", Table2Id = 2})
				.Into(x => x.Table1);

			db.Insert(
				new {Id = 1, Name = "Name21"},
				new {Id = 2, Name = "Name22"})
				.Into(x => x.Table2);
		};

		Because of = () =>
		{
			results = (from record1 in db.Query(x => x.Table1)
					   join record2 in db.Query(x => x.Table2) on record1.Table2Id equals record2.Id
					   select new {Name1 = record1.Name, Name2 = record2.Name}).ToList();

			//results = (from record1 in db.Query(x => x.Table1)
			//           join record2 in db.Query(x => x.Table2) on new {record1.Table2Id, record1.WTF} equals new {record2.Id, record2.WTF}
			//           select new {Name1 = record1.Name, Name2 = record2.Name}).ToList();

			//results = (db.Query(x => x.Table1)
			//    .Join(db.Query(x => x.Table2), record1 => record1.Table2Id, record2 => record2.Id, (record1, record2) => new {record1, record2})
			//    .Join(db.Query(x => x.Table3), join1 => join1.record1.Table3Id, record3 => record3.Id, (join1, record3) => new {Name1 = join1.record1.Name, Name2 = join1.record2.Name, Name3 = record3.Name})).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Name1).ShouldEqual("Name11");
			((object)results[0].Name2).ShouldEqual("Name21");
			((object)results[1].Name1).ShouldEqual("Name12");
			((object)results[1].Name2).ShouldEqual("Name22");
			((object)results[2].Name1).ShouldEqual("Name13");
			((object)results[2].Name2).ShouldEqual("Name21");
			((object)results[3].Name1).ShouldEqual("Name14");
			((object)results[3].Name2).ShouldEqual("Name22");
		};

		It should_retrieve_4_records = () =>
			results.Count.ShouldEqual(4);
	}
	
	public class When_joining_two_tables_on_multiple_conditions
	{
		private static DB db;
		private static IList<long> results;

		private enum Status
		{
			SomeStatus1 = 1,
			SomeStatus2 = 2
		}

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table1] ([Id] INTEGER PRIMARY KEY, [Name] TEXT, [Status] INTEGER);

CREATE TABLE [Table2] ([Id] INTEGER PRIMARY KEY, [Name] TEXT, [Status] INTEGER);"
				);

			db.Insert(
				new {Id = 1, Name = "Name11", Status = Status.SomeStatus1},
				new {Id = 2, Name = "Name12", Status = Status.SomeStatus2},
				new {Id = 3, Name = "Name13", Status = Status.SomeStatus1},
				new {Id = 4, Name = "Name14", Status = Status.SomeStatus2})
				.Into(x => x.Table1);

			db.Insert(
				new {Id = 1, Name = "Name11", Status = Status.SomeStatus1},
				new {Id = 2, Name = "Name12", Status = Status.SomeStatus1},
				new {Id = 3, Name = "Name13", Status = Status.SomeStatus2},
				new {Id = 4, Name = "Name14", Status = Status.SomeStatus2})
				.Into(x => x.Table2);
		};

		Because of = () =>
		{
			results = (from record1 in db.Query(x => x.Table1)
			           join record2 in db.Query(x => x.Table2) on new {record1.Name, record1.Status} equals new {record2.Name, record2.Status}
			           select record1.Id).Cast<long>().ToList();
		};

		It should_retrieve_the_records = () =>
		{
			results[0].ShouldEqual(1);
			results[1].ShouldEqual(4);
		};

		It should_retrieve_4_records = () =>
			results.Count.ShouldEqual(2);
	}
}
