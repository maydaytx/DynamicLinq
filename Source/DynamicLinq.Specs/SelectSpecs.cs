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
		public static Func<IDbConnection> CreateInMemoryDatabase(string sql, params Tuple<string, object>[] parameters)
		{
			return () =>
			{
				SQLiteConnection connection = new SQLiteConnection("Data source=:memory:");

				connection.Open();

				using (SQLiteCommand command = connection.CreateCommand())
				{
					command.CommandText = sql;

					foreach (Tuple<string, object> parameter in parameters)
					{
						IDbDataParameter dataParameter = command.CreateParameter();

						dataParameter.ParameterName = parameter.Item1;
						dataParameter.Value = parameter.Item2;

						command.Parameters.Add(dataParameter);
					}

					command.ExecuteNonQuery();
				}

				return connection;
			};
		}
	}

	public class When_selecting_a_single_column
	{
		private static dynamic db;
		private static IList<string> results;

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
			results = (from record in (object)db.Table
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
											   select (int)record.Id).ToList());
		};

		It should_not_allow = () =>
			exception.ShouldNotBeNull();
	}

	public class When_selecting_and_explicitly_converting_a_single_column
	{
		private static dynamic db;
		private static IList<int> results;

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
			results = (from record in (object)db.Table
					   select record.Id.To<int>()).Cast<int>().ToList();
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);
	}

	public class When_selecting_and_explicitly_converting_a_single_column_that_can_be_null
	{
		private static dynamic db;
		private static IList<int?> results;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Value]);

INSERT INTO [Table] ([Id]) VALUES (1);
INSERT INTO [Table] ([Id], [Value]) VALUES (2, 1);
INSERT INTO [Table] ([Id]) VALUES (3);"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			results = (from record in (object)db.Table
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
		private static dynamic db;
		private static IList<string> results;

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
			results = (from record in (object)db.Table
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
		private static dynamic db;
		private static IList<dynamic> results;

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
			results = (from record in (object)db.Table
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

		It should_retrieve_2_columns = () =>
			((object)results[0]).GetType().GetProperties().Length.ShouldEqual(2);
	}

	public class When_selecting_specific_columns
	{
		private static dynamic db;
		private static IList<dynamic> results;

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
			results = (from record in (object)db.Table
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

		It should_retrieve_2_columns = () =>
			((object)results[0]).GetType().GetProperties().Length.ShouldEqual(2);
	}

	public class When_selecting_specific_columns_where_one_can_be_null
	{
		private static dynamic db;
		private static IList<dynamic> results;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Value], [Name]);

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Name1');
INSERT INTO [Table] ([Id], [Value], [Name]) VALUES (2, 1, 'Name2');
INSERT INTO [Table] ([Id], [Value], [Name]) VALUES (3, 3, 'Name3');"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			results = (from record in (object)db.Table
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
		private static dynamic db;
		private static IList<dynamic> results;

		public enum Status
		{
			SomeStatus1 = 1,
			SomeStatus2 = 2,
			SomeStatus3 = 3
		}

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name], [Status]);

INSERT INTO [Table] ([Id], [Name], [Status]) VALUES (1, 'Name1', 1);
INSERT INTO [Table] ([Id], [Name], [Status]) VALUES (2, 'Name2', 2);
INSERT INTO [Table] ([Id], [Name], [Status]) VALUES (3, 'Name3', 3);"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			results = (from record in (object)db.Table
					   select new { record.Id, Status = record.Status.To<Status>() }).ToList();
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

		It should_retrieve_2_columns = () =>
			((object)results[0]).GetType().GetProperties().Length.ShouldEqual(2);
	}

	public class When_selecting_an_expression_with_a_supplied_name
	{
		private static dynamic db;
		private static IList<dynamic> results;

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
			results = (from record in (object)db.Table
					   select new { Name = "" + record.FirstName + " " + record.LastName }).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)results[0].Name).ShouldEqual("First1 Last1");
			((object)results[1].Name).ShouldEqual("First2 Last2");
			((object)results[2].Name).ShouldEqual("First3 Last3");
		};

		It should_retrieve_3_records = () =>
			results.Count.ShouldEqual(3);

		It should_retrieve_1_columns = () =>
			((object)results[0]).GetType().GetProperties().Length.ShouldEqual(1);
	}

	public class When_selecting_a_byte_array
	{
		private static dynamic db;
		private static IList<dynamic> results;
		private static byte[] bytes;

		Establish context = () =>
		{
			bytes = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };

			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Data]);

INSERT INTO [Table] ([Id], [Data]) VALUES (1, @p0);"
			, new Tuple<string, object>("@p0", bytes));

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			results = (from record in (object)db.Table
					   select new { record.Data }).ToList();
		};

		It should_retrieve_the_records = () =>
			((byte[])results[0].Data).SequenceEqual(bytes);

		It should_retrieve_1_record = () =>
			results.Count.ShouldEqual(1);
	}
}
