using DynamicLinq.Collections;
using DynamicLinq.Dialects;

namespace DynamicLinq.ClauseItems
{
	public class Column : ClauseItem
	{
		private readonly string tableName;
		private readonly string name;

		internal string Name
		{
			get { return name; }
		}

		internal Column(string tableName, string name)
		{
			this.tableName = tableName;
			this.name = name;
		}

		public override bool ShouldParenthesize
		{
			get { return false; }
		}

		public override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			return dialect.Column(tableName, name);
		}

		public override string ToString()
		{
			return "[" + tableName + "].[" + name + "]";
		}
	}
}