using System;
using System.Collections.Generic;
using System.Linq;
using DynamicLinq.ClauseItems;
using DynamicLinq.Collections;

namespace DynamicLinq
{
	public class QueryBuilder
	{
		private readonly DB db;
		private readonly string tableName;
		private readonly ParameterNameProvider nameProvider;

		private LinkedListStringBuilder selectSQL;
		private IList<Tuple<string, object>> selectParameters;
		private IDictionary<string, Type> selectConversions;
		private bool isSingleColumnSelect;

		private ClauseItem whereClause;

		internal QueryBuilder(DB db, string tableName)
		{
			this.db = db;
			this.tableName = tableName;

			nameProvider = new ParameterNameProvider();
		}

		internal void WithWhereClause(Func<dynamic, object> predicate, ClauseGetter clauseGetter)
		{
			object obj = predicate(clauseGetter);

			if (!(obj is ClauseItem))
				throw new ArgumentException("Invalid predicate");

			ClauseItem clauseItem = (ClauseItem) obj;

			if (ReferenceEquals(whereClause, null))
				whereClause = clauseItem;
			else
				whereClause = new BinaryOperation(SimpleOperator.And, whereClause, clauseItem);
		}

		internal void WithSelector(Func<dynamic, object> selector, ClauseGetter clauseGetter)
		{
			selectParameters = new List<Tuple<string, object>>();
			selectConversions = new Dictionary<string, Type>();

			object obj = selector(clauseGetter);

			if (obj == clauseGetter)
			{
				isSingleColumnSelect = false;

				selectSQL = "*";
			}
			else if (obj is ClauseItem)
			{
				isSingleColumnSelect = true;

				ClauseItem item = CheckForConversion(new Tuple<string, ClauseItem>("Column", (ClauseItem) obj), selectConversions);

				selectSQL = item.BuildClause(db.Dialect, selectParameters, nameProvider);
			}
			else
			{
				isSingleColumnSelect = false;

				selectSQL = new LinkedListStringBuilder();

				bool notFirst = false;

				foreach (Tuple<string, ClauseItem> property in (Enumerable.Select(obj.GetType().GetProperties(), p => new Tuple<string, ClauseItem>(p.Name, (ClauseItem) p.GetValue(obj, null)))))
				{
					if (notFirst)
						selectSQL.Append(", ");
					else
						notFirst = true;

					ClauseItem item = CheckForConversion(property, selectConversions);

					selectSQL.Append(item.BuildClause(db.Dialect, selectParameters, nameProvider));
					selectSQL.Append(" AS [" + property.Item1 + "]");
				}
			}
		}

		internal QueryInfo Build()
		{
			LinkedListStringBuilder sql = "SELECT ";

			if (selectSQL == null)
			{
				isSingleColumnSelect = false;

				selectSQL = "*";
			}

			sql.Append(selectSQL);

			IList<Tuple<string, object>> whereParameters = new List<Tuple<string, object>>();

			sql.Append(" FROM [" + tableName + "]");

			if (!ReferenceEquals(whereClause, null))
			{
				LinkedListStringBuilder whereSQL = whereClause.BuildClause(db.Dialect, whereParameters, nameProvider);

				sql.Append(" WHERE ");
				sql.Append(whereSQL);
			}

			return new QueryInfo(sql.ToString(), selectParameters.Concat(whereParameters), selectConversions, isSingleColumnSelect);
		}

		private static ClauseItem CheckForConversion(Tuple<string, ClauseItem> property, IDictionary<string, Type> conversions)
		{
			ClauseItem item = property.Item2;

			if (item is ConvertOperation)
			{
				ConvertOperation convert = (ConvertOperation)item;

				conversions.Add(property.Item1, convert.Type);

				item = convert.Item;
			}

			return item;
		}
	}
}
