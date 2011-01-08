using System;
using System.Collections.Generic;
using System.Linq;
using DynamicLinq.Databases;
using Machine.Specifications;

namespace DynamicLinq
{
	public class When_predicating_on_a_boolean_column
	{
		private static DB db;
		private static IList<long> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [IsSomething] INTEGER)");

			db.Insert(
				new {Id = 1, IsSomething = 1},
				new {Id = 2, IsSomething = 0},
				new {Id = 3, IsSomething = 1},
				new {Id = 4, IsSomething = 0})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   where record.IsSomething == true
					   select record.Id).Cast<long>().ToList();
		};

		It should_retrieve_the_records = () =>
		{
			results[0].ShouldEqual(1L);
			results[1].ShouldEqual(3L);
		};

		It should_retrieve_2_records = () =>
			results.Count.ShouldEqual(2);
	}

	public class When_predicating_on_a_clause
	{
		private static DB db;
		private static IList<long> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [FirstName] TEXT, [LastName] TEXT)");

			db.Insert(
				new {Id = 1, FirstName = "John", LastName = "Johnson"},
				new {Id = 2, FirstName = "Bob", LastName = "Bobson"},
				new {Id = 3, FirstName = "John", LastName = "Bobson"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   where record.FirstName != "John"
					   select record.Id).Cast<long>().ToList();
		};

		It should_retrieve_the_records = () =>
			results[0].ShouldEqual(2L);

		It should_retrieve_1_record = () =>
			results.Count.ShouldEqual(1);
	}

	public class When_predicating_on_a_like_clause
	{
		private static DB db;
		private static IList<long> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Sal"},
				new {Id = 2, Name = "Bob"},
				new {Id = 3, Name = "Joe"},
				new {Id = 4, Name = "Sally"})
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   where record.Name.Like("Sal%")
					   select record.Id).Cast<long>().ToList();
		};

		It should_retrieve_the_records = () =>
		{
			results[0].ShouldEqual(1L);
			results[1].ShouldEqual(4L);
		};

		It should_retrieve_2_records = () =>
			results.Count.ShouldEqual(2);
	}

	public class When_predicating_on_a_clause_with_external_functions
	{
		private static DB db;
		private static Exception exception;

		private static bool IsSomething(dynamic obj)
		{
			return true;
		}

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Sal"},
				new {Id = 2, Name = "Bob"},
				new {Id = 3, Name = "Joe"},
				new {Id = 4, Name = "Sally"})
				.Into(x => x.Table);
		};

		private Because of = () =>
		{
			exception = Catch.Exception(() => (from record in db.Query(x => x.Table)
											   where IsSomething(record.Name)
											   select record).ToList());
		};

		It should_not_allow = () =>
			exception.ShouldNotBeNull();
	}

	public class When_predicating_on_an_invalid_clause
	{
		private static DB db;
		private static Exception exception;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Sal"},
				new {Id = 2, Name = "Bob"},
				new {Id = 3, Name = "Joe"},
				new {Id = 4, Name = "Sally"})
				.Into(x => x.Table);
		};

		private Because of = () =>
		{
			exception = Catch.Exception(() => (from record in db.Query(x => x.Table)
											   where record
											   select record).ToList());
		};

		It should_not_allow = () =>
			exception.ShouldNotBeNull();
	}

	public class When_predicating_on_multiple_clauses
	{
		private static DB db;
		private static IList<long> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Sal"},
				new {Id = 2, Name = "Bob"},
				new {Id = 3, Name = "Joe"},
				new {Id = 4, Name = "Sally"})
				.Into(x => x.Table);
		};

		private Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   where record.Name.Like("S%")
					   where record.Id > 1
					   select record.Id).Cast<long>().ToList();
		};

		It should_retrieve_the_records = () =>
			results[0].ShouldEqual(4L);

		It should_retrieve_1_records = () =>
			results.Count.ShouldEqual(1);
	}

	public class When_comparing_something_to_null
	{
		private static DB db;
		private static IList<long> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new {Id = 1, Name = "Sal"},
				new {Id = 2, Name = "Bob"},
				new {Id = 3, Name = "Joe"},
				new {Id = 4})
				.Into(x => x.Table);
		};

		private Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   where record.Name == null
					   select record.Id).Cast<long>().ToList();
		};

		It should_retrieve_the_records = () =>
			results[0].ShouldEqual(4L);

		It should_retrieve_1_records = () =>
			results.Count.ShouldEqual(1);
	}

	public class When_comparing_something_to_an_enum
	{
		private static DB db;
		private static IList<long> results;

		private enum Status
		{
			SomeStatus1 = 1,
			SomeStatus2 = 2,
			SomeStatus3 = 3
		}

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT, [Status] INTEGER)");

			db.Insert(
				new {Id = 1, Name = "Sal", Status = Status.SomeStatus1},
				new {Id = 2, Name = "Bob", Status = Status.SomeStatus2},
				new {Id = 3, Name = "Joe", Status = Status.SomeStatus3},
				new {Id = 4, Name = "Sally"})
				.Into(x => x.Table);
		};

		private Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   where record.Status == (int) Status.SomeStatus2
					   select record.Id).Cast<long>().ToList();
		};

		It should_retrieve_the_records = () =>
			results[0].ShouldEqual(2L);

		It should_retrieve_1_records = () =>
			results.Count.ShouldEqual(1);
	}

	public class When_comparing_a_date_time
	{
		private static DB db;
		private static IList<long> results;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT, [Date] DATETIME)");

			db.Insert(
				new {Id = 1, Name = "Sal", Date = new DateTime(2012, 12, 21)},
				new {Id = 2, Name = "Bob", Date = new DateTime(2001, 09, 11)},
				new {Id = 3, Name = "Joe", Date = new DateTime(1776, 07, 04)},
				new {Id = 4, Name = "Sally"})
				.Into(x => x.Table);
		};

		private Because of = () =>
		{
			results = (from record in db.Query(x => x.Table)
					   where record.Date > DateTime.Now
					   select record.Id).Cast<long>().ToList();
		};

		It should_retrieve_the_records = () =>
			results[0].ShouldEqual(1L);

		It should_retrieve_1_records = () =>
			results.Count.ShouldEqual(1);
	}
}
