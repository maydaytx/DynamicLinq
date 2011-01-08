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

		private string joinTableName;
		private LinkedListStringBuilder joinSQL;
		private IList<Tuple<string, object>> joinParameters;

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
			object obj = selector(clauseGetter);

			SetSelectSQL(obj, clauseGetter);
		}

		public void WithJoin(Func<object, object> outerKeySelector, Func<object, object> innerKeySelector, Func<object, object, object> resultSelector, ClauseGetter outerClauseGetter, ClauseGetter innerClauseGetter, string innerTableName)
		{
			object outerKey = outerKeySelector(outerClauseGetter);
			object innerKey = innerKeySelector(innerClauseGetter);

			joinTableName = innerTableName;
			joinParameters = new List<Tuple<string, object>>();
			ClauseItem joinClause;

			if (outerKey == outerClauseGetter || innerKey == innerClauseGetter)
			{
				throw new ArgumentException("Invalid key selector");
			}
			else if (outerKey is ClauseItem && innerKey is ClauseItem)
			{
				joinClause = new BinaryOperation(SimpleOperator.Equal, (ClauseItem) outerKey, (ClauseItem) innerKey);
			}
			else
			{
				ClauseItem[] outerKeyProperties = GetClauseItems(outerKey).Select(property => property.Item2).ToArray();
				ClauseItem[] innerKeyProperties = GetClauseItems(innerKey).Select(property => property.Item2).ToArray();

				if (outerKeyProperties.Length == 0 || innerKeyProperties.Length == 0)
					throw new ArgumentException("Invalid key selector");

				if (outerKeyProperties.Length != innerKeyProperties.Length)
					throw new ArgumentException("Unequal number of selectors");

				joinSQL = new LinkedListStringBuilder();

				joinClause = new BinaryOperation(SimpleOperator.Equal, outerKeyProperties[0], innerKeyProperties[0]);

				for (int i = 1; i < outerKeyProperties.Length; ++i)
				{
					ClauseItem additionalClause = new BinaryOperation(SimpleOperator.Equal, outerKeyProperties[i], innerKeyProperties[i]);
					joinClause = new BinaryOperation(SimpleOperator.And, joinClause, additionalClause);
				}
			}
			
			joinSQL = joinClause.BuildClause(db.Dialect, joinParameters, nameProvider);

			object obj = resultSelector(outerClauseGetter, innerClauseGetter);

			SetSelectSQL(obj, outerClauseGetter, innerClauseGetter);
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

			sql.Append(" FROM [" + tableName + "]");

			if (joinSQL != null)
			{
				sql.Append(" INNER JOIN [" + joinTableName + "] ON ");

				sql.Append(joinSQL);
			}

			IList<Tuple<string, object>> whereParameters = new List<Tuple<string, object>>();

			if (!ReferenceEquals(whereClause, null))
			{
				LinkedListStringBuilder whereSQL = whereClause.BuildClause(db.Dialect, whereParameters, nameProvider);

				sql.Append(" WHERE ");
				sql.Append(whereSQL);
			}

			IEnumerable<Tuple<string, object>> parameters = selectParameters.Concat(whereParameters);

			if (joinSQL != null)
				parameters = parameters.Concat(joinParameters);

			return new QueryInfo(sql.ToString(), parameters, selectConversions, isSingleColumnSelect);
		}

		private void SetSelectSQL(object obj, params ClauseGetter[] clauseGetters)
		{
			selectParameters = new List<Tuple<string, object>>();
			selectConversions = new Dictionary<string, Type>();

			if (Enumerable.Contains(clauseGetters, obj))
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

				foreach (Tuple<string, ClauseItem> property in GetClauseItems(obj))
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

		private static IEnumerable<Tuple<string, ClauseItem>> GetClauseItems(object obj)
		{
			return Enumerable.Select(obj.GetType().GetProperties(), p => new Tuple<string, ClauseItem>(p.Name, (ClauseItem) p.GetValue(obj, null)));
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
