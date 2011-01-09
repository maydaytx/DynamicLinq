using System.Collections.Generic;
using System.Linq;
using DynamicLinq.Databases;
using Machine.Specifications;

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
}
