using DynamicLinq.Databases;
using Machine.Specifications;

namespace DynamicLinq
{
	public class When_counting_rows
	{
		private static DB db;
		private static int result;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new { Id = 1, Name = "Name1" },
				new { Id = 2, Name = "Name2" },
				new { Id = 3, Name = "Name3" },
				new { Id = 4, Name = "Name4" })
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			result = (from record in db.Query(x => x.Table)
					  select record).Count();
		};

		It should_count_all_records = () =>
			result.ShouldEqual(4);
	}

	public class When_counting_rows_with_where_clause
	{
		private static DB db;
		private static int result;

		Establish context = () =>
		{
			db = SQLite.GetDB("CREATE TABLE [Table] ([Id] INTEGER PRIMARY KEY, [Name] TEXT)");

			db.Insert(
				new { Id = 1, Name = "Name1" },
				new { Id = 2, Name = "Name2" },
				new { Id = 3, Name = "Name3" },
				new { Id = 4, Name = "Name4" })
				.Into(x => x.Table);
		};

		Because of = () =>
		{
			result = (from record in db.Query(x => x.Table)
					  where record.Id > 1 && record.Id < 4
					  select record).Count();
		};

		It should_count_all_records = () =>
			result.ShouldEqual(2);
	}
}
