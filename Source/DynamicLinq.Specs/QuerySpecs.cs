using System;
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

	public class When_querying_all_records_in_a_table
	{
		private static dynamic db;
		private static IList<dynamic> result;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name]);

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Record1');
INSERT INTO [Table] ([Id], [Name]) VALUES (2, 'Record2');
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Record3');"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			result = (from record in (object) db.Table
			          select record).ToList();
		};

		It should_retrieve_the_records = () =>
		{
			((object)result[0].Id).ShouldEqual(1L);
			((object)result[0].Name).ShouldEqual("Record1");
			((object)result[1].Id).ShouldEqual(2L);
			((object)result[1].Name).ShouldEqual("Record2");
			((object)result[2].Id).ShouldEqual(3L);
			((object)result[2].Name).ShouldEqual("Record3");
		};

		It should_retrieve_3_records = () =>
			result.Count.ShouldEqual(3);
	}
}
