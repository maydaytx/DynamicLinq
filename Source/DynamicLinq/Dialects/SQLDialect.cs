using System;
using System.Collections.Generic;
using System.Data;
using DynamicLinq.Collections;
using DynamicLinq.InsertUpdates;
using DynamicLinq.Queries;

namespace DynamicLinq.Dialects
{
	public abstract class SQLDialect : IDialect
	{
		private readonly Func<IDbConnection> getConnection;

		protected SQLDialect(Func<IDbConnection> getConnection)
		{
			this.getConnection = getConnection;
		}

		public virtual string AddOperator
		{
			get { return "+"; }
		}

		public virtual string ConcatenateOperator
		{
			get { return "||"; }
		}

		public virtual void ConcatenateStrings(LinkedListStringBuilder builder, LinkedListStringBuilder appendee)
		{
			builder.Append(ConcatenateOperator);
			builder.Append(appendee);
		}

		public virtual string SubtractOperator
		{
			get { return "-"; }
		}

		public virtual string MultiplyOperator
		{
			get { return "*"; }
		}

		public virtual string DivideOperator
		{
			get { return "/"; }
		}

		public virtual string ModOperator
		{
			get { return "%"; }
		}

		public virtual string AndOperator
		{
			get { return "AND"; }
		}

		public virtual string EqualOperator
		{
			get { return "="; }
		}

		public virtual string NotEqualOperator
		{
			get { return "<>"; }
		}

		public virtual string CompareEqualToNull
		{
			get { return " IS NULL"; }
		}

		public virtual string CompareNotEqualToNull
		{
			get { return " IS NOT NULL"; }
		}

		public virtual string OrOperator
		{
			get { return "OR"; }
		}

		public virtual string LessThanOperator
		{
			get { return "<"; }
		}

		public virtual string GreaterThanOperator
		{
			get { return ">"; }
		}

		public virtual string LessThanOrEqualOperator
		{
			get { return "<="; }
		}

		public virtual string GreaterThanOrEqualOperator
		{
			get { return ">="; }
		}

		public virtual string LikeOperator
		{
			get { return "LIKE"; }
		}

		public virtual string PositiveOperator
		{
			get { return "+"; }
		}

		public virtual string NegativeOperator
		{
			get { return "-"; }
		}

		public virtual string NotOperator
		{
			get { return "NOT"; }
		}

		public virtual string ComplementOperator
		{
			get { return "~"; }
		}

		public virtual void InOperator(LinkedListStringBuilder builder, IEnumerable<LinkedListStringBuilder> list)
		{
			builder.Append(" IN (");

			bool first = true;

			foreach (LinkedListStringBuilder item in list)
			{
				if (first)
					first = false;
				else
					builder.Append(", ");

				builder.Append(item);
			}

			builder.Append(")");
		}

		public virtual string DateTimeFormat
		{
			get { return "yyyy-MM-dd HH:mm:ss.fffffff"; }
		}

		public virtual string ParameterPrefix
		{
			get { return "@"; }
		}

		public virtual void SkipTakeClause(LinkedListStringBuilder builder, int? skipCount, int? takeCount)
		{
			builder.Append(" LIMIT ");

			if (skipCount != null && takeCount != null)
				builder.Append(skipCount + ", " + takeCount);
			else if (skipCount != null)
				builder.Append(skipCount + ", " + int.MaxValue);
			else if (takeCount != null)
				builder.Append(takeCount.ToString());
		}

		public virtual string CountColumn
		{
			get { return "COUNT(*)"; }
		}

		public virtual string Constant(object value, ParameterCollection parameters)
		{
			if (value is string || value is byte[] || value.GetType().IsEnum)
			{
				return parameters.Add(value);
			}
			else if (value is char)
			{
				return "'" + value + "'";
			}
			else if (value is DateTime)
			{
				return "'" + ((DateTime) value).ToString(DateTimeFormat) + "'";
			}
			else if (value is bool)
			{
				return (bool) value ? "1" : "0";
			}
			else
			{
				return value.ToString();
			}
		}

		internal IDbConnection GetConnection()
		{
			return getConnection();
		}

		public IQueryBuilder GetQueryBuilder(string tableName)
		{
			return new SQLQueryBuilder(this, tableName);
		}

		public QueryConnection GetConnection(QueryInfo queryInfo)
		{
			return new SQLQueryConnection(this, queryInfo);
		}

		public IInsertor GetInsertor(object[] rows)
		{
			return new SQLInsertor(this, rows);
		}

		public IUpdator GetUpdator(string tableName)
		{
			return new SQLUpdator(this, tableName);
		}
	}
}
