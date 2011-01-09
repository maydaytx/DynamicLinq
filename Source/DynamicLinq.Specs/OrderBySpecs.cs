using System.Collections.Generic;
using System.Linq;
using DynamicLinq.Databases;
using Machine.Specifications;

namespace DynamicLinq
{
	public class When_ordering_by_a_column_ascending
	{
		private static DB db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new { Id = 1, Name = "Name2" },
				new { Id = 2, Name = "Name1" },
				new { Id = 3, Name = "Name3" })
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   orderby record.Name ascending
					   select record).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Id).ShouldEqual(2L);
			((object)results[0].Name).ShouldEqual("Name1");
			((object)results[1].Id).ShouldEqual(1L);
			((object)results[1].Name).ShouldEqual("Name2");
			((object)results[2].Id).ShouldEqual(3L);
			((object)results[2].Name).ShouldEqual("Name3");
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}
	public class When_ordering_by_a_column_descending
	{
		private static DB db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new { Id = 1, Name = "Name2" },
				new { Id = 2, Name = "Name1" },
				new { Id = 3, Name = "Name3" })
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   orderby record.Name descending
					   select record).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Id).ShouldEqual(3L);
			((object)results[0].Name).ShouldEqual("Name3");
			((object)results[1].Id).ShouldEqual(1L);
			((object)results[1].Name).ShouldEqual("Name2");
			((object)results[2].Id).ShouldEqual(2L);
			((object)results[2].Name).ShouldEqual("Name1");
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}
}
