using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Brawndo.DynamicLinq
{
	public class When_inserting_records_into_a_table
	{
		private static dynamic db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] VARCHAR(256), [Value] INTEGER);"
				);
		};

		Because of = () =>
		{
			((object) db.Table).Insert(
				new {Name = "Name1", Value = 1},
				new {Name = "Name2", Value = 2},
				new {Name = "Name3", Value = 3});

			results = (from record in (object) db.Table
			           select record).ToList();
		};

		It should_insert_the_records = () =>
		{
			((object)results[0].Id).ShouldEqual(1L);
			((object)results[0].Name).ShouldEqual("Name1");
			((object)results[0].Value).ShouldEqual(1L);
			((object)results[1].Id).ShouldEqual(2L);
			((object)results[1].Name).ShouldEqual("Name2");
			((object)results[1].Value).ShouldEqual(2L);
			((object)results[2].Id).ShouldEqual(3L);
			((object)results[2].Name).ShouldEqual("Name3");
			((object)results[2].Value).ShouldEqual(3L);
		};

		It should_insert_3_records = () =>
			results.Count.ShouldEqual(3);
	}
	
	public class When_updating_records_in_a_table
	{
		private static dynamic db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] VARCHAR(256), [Value] INTEGER);"
				);
		};

		Because of = () =>
		{
			((object) db.Table).Insert(
				new {Name = "Name1", Value = 1},
				new {Name = "Name2", Value = 2},
				new {Name = "Name3", Value = 3});

			((object) db.Table)
				.Update(new {Name = "Name4", Value = 4})
				.Where(t => t.Id == 1)
				.Execute();

			results = (from record in (object) db.Table
			           select record).ToList();
		};

		It should_insert_the_records = () =>
		{
			((object)results[0].Id).ShouldEqual(1L);
			((object)results[0].Name).ShouldEqual("Name4");
			((object)results[0].Value).ShouldEqual(4L);
			((object)results[1].Id).ShouldEqual(2L);
			((object)results[1].Name).ShouldEqual("Name2");
			((object)results[1].Value).ShouldEqual(2L);
			((object)results[2].Id).ShouldEqual(3L);
			((object)results[2].Name).ShouldEqual("Name3");
			((object)results[2].Value).ShouldEqual(3L);
		};
	}
}
