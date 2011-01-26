using System;
using DynamicLinq.Dialects;
using DynamicLinq.InsertUpdates;
using DynamicLinq.Queries;

namespace DynamicLinq
{
	public class DB
	{
		private readonly IDialect dialect;

		public DB(IDialect dialect)
		{
			this.dialect = dialect;
		}

		public Query Query(Func<dynamic, object> getTableName)
		{
			NameGetter nameGetter = new NameGetter();

			string tableName = (string) getTableName(nameGetter);

			return new Query(dialect, tableName);
		}

		public IInsertor Insert(params object[] rows)
		{
			return dialect.GetInsertor(rows);
		}

		public IUpdator Update(Func<dynamic, object> getTableName)
		{
			NameGetter nameGetter = new NameGetter();

			string tableName = (string) getTableName(nameGetter);

			return dialect.GetUpdator(tableName);
		}
	}
}
