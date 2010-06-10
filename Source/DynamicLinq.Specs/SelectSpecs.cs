﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Machine.Specifications;

namespace DynamicLinq.Specs
{
	internal static class SQLite
	{
		public static Func<IDbConnection> CreateInMemoryDatabase(string sql)
		{
			return () =>
			{
				SQLiteConnection connection = new SQLiteConnection("Data source=:memory:");

				connection.Open();

				using (SQLiteCommand command = connection.CreateCommand())
				{
					command.CommandText = sql;

					command.ExecuteNonQuery();
				}

				return connection;
			};
		}
	}

	public class When_selecting_a_single_column
	{
		private static dynamic db;
		private static IList<string> result;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name]);

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Name1');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Name2');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Name3');"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			result = (from record in (object)db.Table
					  select record.Name).Cast<string>().ToList();
		};

		It should_retrieve_the_records = () =>
		{
			result[0].ShouldEqual("Name1");
			result[1].ShouldEqual("Name2");
			result[2].ShouldEqual("Name3");
		};

		It should_retrieve_3_records = () =>
			result.Count.ShouldEqual(3);
	}

	public class When_selecting_and_casting_a_single_column
	{
		private static dynamic db;
		private static Exception exception;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name]);

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Name1');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Name2');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Name3');"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			exception = Catch.Exception(() => (from record in (object)db.Table
			                                   select (int) record.Id).ToList());
		};

		It should_not_allow = () =>
			exception.ShouldNotBeNull();
	}

	public class When_selecting_an_expression_without_a_supplied_name
	{
		private static dynamic db;
		private static IList<string> result;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [FirstName], [LastName]);

INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (1, 'First1', 'Last1');
INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (2, 'First2', 'Last2');
INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (3, 'First3', 'Last3');"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			result = (from record in (object)db.Table
					  select "" + record.FirstName + " " + record.LastName).Cast<string>().ToList();
		};

		It should_retrieve_the_records = () =>
		{
			result[0].ShouldEqual("First1 Last1");
			result[1].ShouldEqual("First2 Last2");
			result[2].ShouldEqual("First3 Last3");
		};

		It should_retrieve_3_records = () =>
			result.Count.ShouldEqual(3);
	}

	public class When_selecting_all_columns
	{
		private static dynamic db;
		private static IList<dynamic> result;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name]);

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Name1');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Name2');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Name3');"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			result = (from record in (object)db.Table
					  select record).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)result[0].Id).ShouldEqual(1L);
			((object)result[0].Name).ShouldEqual("Name1");
			((object)result[1].Id).ShouldEqual(2L);
			((object)result[1].Name).ShouldEqual("Name2");
			((object)result[2].Id).ShouldEqual(3L);
			((object)result[2].Name).ShouldEqual("Name3");
		};

		It should_retrieve_3_records = () =>
			result.Count.ShouldEqual(3);

		It should_retrieve_2_columns = () =>
			((object)result[0]).GetType().GetProperties().Length.ShouldEqual(2);
	}

	public class When_selecting_specific_columns
	{
		private static dynamic db;
		private static IList<dynamic> result;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [FirstName], [LastName]);

INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (1, 'First1', 'Last1');
INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (2, 'First2', 'Last2');
INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (3, 'First3', 'Last3');"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			result = (from record in (object)db.Table
			          select new {record.Id, record.FirstName}).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)result[0].Id).ShouldEqual(1L);
			((object)result[0].FirstName).ShouldEqual("First1");
			((object)result[1].Id).ShouldEqual(2L);
			((object)result[1].FirstName).ShouldEqual("First2");
			((object)result[2].Id).ShouldEqual(3L);
			((object)result[2].FirstName).ShouldEqual("First3");
		};

		It should_retrieve_3_records = () =>
			result.Count.ShouldEqual(3);

		It should_retrieve_2_columns = () =>
			((object)result[0]).GetType().GetProperties().Length.ShouldEqual(2);
	}

	public class When_selecting_an_expression_with_a_supplied_name
	{
		private static dynamic db;
		private static IList<dynamic> result;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
				(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [FirstName], [LastName]);

INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (1, 'First1', 'Last1');
INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (2, 'First2', 'Last2');
INSERT INTO [Table] ([Id], [FirstName], [LastName]) VALUES (3, 'First3', 'Last3');"
				);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			result = (from record in (object)db.Table
			          select new {Name = "" + record.FirstName + " " + record.LastName}).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)result[0].Name).ShouldEqual("First1 Last1");
			((object)result[1].Name).ShouldEqual("First2 Last2");
			((object)result[2].Name).ShouldEqual("First3 Last3");
		};

		It should_retrieve_3_records = () =>
			result.Count.ShouldEqual(3);

		It should_retrieve_1_columns = () =>
			((object)result[0]).GetType().GetProperties().Length.ShouldEqual(1);
	}
}
