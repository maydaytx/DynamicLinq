﻿using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace DynamicLinq.Specs
{
	public class When_predicating_on_a_boolean_column
	{
		private static dynamic db;
		private static IList<long> results;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
				(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [IsSomething]);

INSERT INTO [Table] ([Id], [IsSomething]) VALUES (1, 1);
INSERT INTO [Table] ([Id], [IsSomething]) VALUES (2, 0);
INSERT INTO [Table] ([Id], [IsSomething]) VALUES (3, 1);
INSERT INTO [Table] ([Id], [IsSomething]) VALUES (4, 0);"
				);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			results = (from record in (object)db.Table
					   where record.IsSomething
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
			var getConnection = SQLite.CreateInMemoryDatabase
				(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [FirstName], [LastName]);

INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (1, 'John', 'Johnson');
INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (2, 'Bob', 'Bobson');
INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (3, 'John', 'Bobson');"
				);

			db = new DB(getConnection);
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
			var getConnection = SQLite.CreateInMemoryDatabase
				(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name]);

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Sal');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Bob');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Joe');
INSERT INTO [Table] ([Id], [Name]) VALUES (4, 'Sally');"
				);

			db = new DB(getConnection);
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
			var getConnection = SQLite.CreateInMemoryDatabase
				(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name]);

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Sal');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Bob');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Joe');
INSERT INTO [Table] ([Id], [Name]) VALUES (4, 'Sally');"
				);

			db = new DB(getConnection);
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
			var getConnection = SQLite.CreateInMemoryDatabase
				(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name]);

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Sal');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Bob');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Joe');
INSERT INTO [Table] ([Id], [Name]) VALUES (4, 'Sally');"
				);

			db = new DB(getConnection);
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
			var getConnection = SQLite.CreateInMemoryDatabase
				(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name]);

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Sal');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Bob');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Joe');
INSERT INTO [Table] ([Id], [Name]) VALUES (4, 'Sally');"
				);

			db = new DB(getConnection);
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
			var getConnection = SQLite.CreateInMemoryDatabase
				(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name]);

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Sal');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Bob');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Joe');
INSERT INTO [Table] ([Id]) VALUES (4);"
				);

			db = new DB(getConnection);
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
}
