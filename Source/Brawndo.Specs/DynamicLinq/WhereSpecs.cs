using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Brawndo.DynamicLinq
{
	public class When_predicating_on_a_boolean_column
	{
		private static dynamic db;
		private static IList<long> results;

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [IsSomething] BIT);

INSERT INTO [Table] ([Id], [IsSomething]) VALUES (1, 1);
INSERT INTO [Table] ([Id], [IsSomething]) VALUES (2, 0);
INSERT INTO [Table] ([Id], [IsSomething]) VALUES (3, 1);
INSERT INTO [Table] ([Id], [IsSomething]) VALUES (4, 0);"
				);
		};

		Because of = () =>
		{
			results = (from record in (object)db.Table
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
		private static dynamic db;
		private static IList<long> results;

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [FirstName] VARCHAR(256), [LastName] VARCHAR(256));

INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (1, 'John', 'Johnson');
INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (2, 'Bob', 'Bobson');
INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (3, 'John', 'Bobson');"
				);
		};

		Because of = () =>
		{
			results = (from record in (object)db.Table
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
		private static dynamic db;
		private static IList<long> results;

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] VARCHAR(256));

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Sal');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Bob');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Joe');
INSERT INTO [Table] ([Id], [Name]) VALUES (4, 'Sally');"
				);
		};

		Because of = () =>
		{
			results = (from record in (object)db.Table
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
		private static dynamic db;
		private static Exception exception;

		private static bool IsSomething(dynamic obj)
		{
			return true;
		}

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] VARCHAR(256));

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Sal');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Bob');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Joe');
INSERT INTO [Table] ([Id], [Name]) VALUES (4, 'Sally');"
				);
		};

		private Because of = () =>
		{
			exception = Catch.Exception(() => (from record in (object) db.Table
											   where IsSomething(record.Name)
											   select record).ToList());
		};

		It should_not_allow = () =>
			exception.ShouldNotBeNull();
	}

	public class When_predicating_on_an_invalid_clause
	{
		private static dynamic db;
		private static Exception exception;

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] VARCHAR(256));

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Sal');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Bob');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Joe');
INSERT INTO [Table] ([Id], [Name]) VALUES (4, 'Sally');"
				);
		};

		private Because of = () =>
		{
			exception = Catch.Exception(() => (from record in (object)db.Table
											   where record
											   select record).ToList());
		};

		It should_not_allow = () =>
			exception.ShouldNotBeNull();
	}

	public class When_predicating_on_multiple_clauses
	{
		private static dynamic db;
		private static IList<long> results;

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] VARCHAR(256));

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Sal');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Bob');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Joe');
INSERT INTO [Table] ([Id], [Name]) VALUES (4, 'Sally');"
				);
		};

		private Because of = () =>
		{
			results = (from record in (object) db.Table
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
		private static dynamic db;
		private static IList<long> results;

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] VARCHAR(256));

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Sal');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Bob');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Joe');
INSERT INTO [Table] ([Id]) VALUES (4);"
				);
		};

		private Because of = () =>
		{
			results = (from record in (object)db.Table
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
		private static dynamic db;
		private static IList<long> results;

		public enum Status
		{
			SomeStatus1 = 1,
			SomeStatus2 = 2,
			SomeStatus3 = 3
		}

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] VARCHAR(256), [Status] INTEGER);

INSERT INTO [Table] ([Id], [Name], [Status]) VALUES (1, 'Sal', 1);
INSERT INTO [Table] ([Id], [Name], [Status]) VALUES (2, 'Bob', 2);
INSERT INTO [Table] ([Id], [Name], [Status]) VALUES (3, 'Joe', 3);
INSERT INTO [Table] ([Id], [Name]) VALUES (4, 'Sally');"
				);
		};

		private Because of = () =>
		{
			results = (from record in (object)db.Table
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
		private static dynamic db;
		private static IList<long> results;

		public enum Status
		{
			SomeStatus1 = 1,
			SomeStatus2 = 2,
			SomeStatus3 = 3
		}

		Establish context = () =>
		{
			db = SQLite.GetDB
				(
@"CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] VARCHAR(256), [Date] DATETIME);

INSERT INTO [Table] ([Id], [Name], [Date]) VALUES (1, 'Sal', '2012-12-21');
INSERT INTO [Table] ([Id], [Name], [Date]) VALUES (2, 'Bob', '2001-09-11');
INSERT INTO [Table] ([Id], [Name], [Date]) VALUES (3, 'Joe', '1776-07-04');
INSERT INTO [Table] ([Id], [Name]) VALUES (4, 'Sally');"
				);
		};

		private Because of = () =>
		{
			results = (from record in (object)db.Table
					   where record.Date > DateTime.Now
					   select record.Id).Cast<long>().ToList();
		};

		It should_retrieve_the_records = () =>
			results[0].ShouldEqual(1L);

		It should_retrieve_1_records = () =>
			results.Count.ShouldEqual(1);
	}
}
