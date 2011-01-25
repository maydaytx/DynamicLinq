﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DynamicLinq.ClauseItems;
using DynamicLinq.Collections;

namespace DynamicLinq.Queries
{
	internal class QueryBuilder
	{
		private readonly Dialect dialect;
		private readonly string tableName;
		private readonly ParameterNameProvider nameProvider;
		
		private LinkedListStringBuilder selectSQL;
		private ParameterCollection selectParameters;
		private IDictionary<string, Type> selectConversions;
		private bool isSingleColumnSelect;
		private bool isCountSelector;

		private ClauseItem whereClause;

		private string joinTableName;
		private LinkedListStringBuilder joinSQL;
		private ParameterCollection joinParameters;

		private readonly IList<Tuple<ClauseItem, ListSortDirection>> orderByClauses;

		private int? skipCount;
		private int? takeCount;

		internal QueryBuilder(Dialect dialect, string tableName)
		{
			this.dialect = dialect;
			this.tableName = tableName;

			nameProvider = new ParameterNameProvider(dialect);
			orderByClauses = new List<Tuple<ClauseItem, ListSortDirection>>();
		}

		internal void AddWhereClause(Func<object, object> predicate, ClauseGetter clauseGetter)
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

		internal void WithSelector(Func<object, object> selector, ClauseGetter clauseGetter)
		{
			object obj = selector(clauseGetter);

			SetSelectSQL(obj, clauseGetter);
		}

		internal void WithJoin(Func<object, object> outerKeySelector, Func<object, object> innerKeySelector, Func<object, object, object> resultSelector, ClauseGetter outerClauseGetter, ClauseGetter innerClauseGetter, string innerTableName)
		{
			object outerKey = outerKeySelector(outerClauseGetter);
			object innerKey = innerKeySelector(innerClauseGetter);

			joinTableName = innerTableName;
			joinParameters = new ParameterCollection(nameProvider);
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
			
			joinSQL = joinClause.BuildClause(dialect, joinParameters);

			object obj = resultSelector(outerClauseGetter, innerClauseGetter);

			SetSelectSQL(obj, outerClauseGetter, innerClauseGetter);
		}

		internal void AddOrderBy(Func<object, object> keySelector, ClauseGetter clauseGetter, ListSortDirection sortDirection)
		{
			object obj = keySelector(clauseGetter);

			if (!(obj is ClauseItem))
				throw new ArgumentException("Invalid key selector");

			ClauseItem clauseItem = (ClauseItem) obj;

			orderByClauses.Add(new Tuple<ClauseItem, ListSortDirection>(clauseItem, sortDirection));
		}

		internal void AddSkip(int count)
		{
			if (skipCount != null)
				skipCount += count;
			else
				skipCount = count;
		}

		internal void SetTake(int count)
		{
			takeCount = count;
		}

		internal void SetCountSelector()
		{
			isCountSelector = true;
		}

		internal QueryInfo Build()
		{
			LinkedListStringBuilder sql = "SELECT ";

			if (isCountSelector)
			{
				selectParameters = new ParameterCollection(nameProvider);
				selectConversions = new Dictionary<string, Type>();

				isSingleColumnSelect = true;

				selectSQL = dialect.CountColumn;
			}
			else if (selectSQL == null)
			{
				selectParameters = new ParameterCollection(nameProvider);
				selectConversions = new Dictionary<string, Type>();

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

			ParameterCollection whereParameters = new ParameterCollection(nameProvider);

			if (!ReferenceEquals(whereClause, null))
			{
				LinkedListStringBuilder whereSQL = whereClause.BuildClause(dialect, whereParameters);

				sql.Append(" WHERE ");
				sql.Append(whereSQL);
			}

			ParameterCollection orderByParameters = new ParameterCollection(nameProvider);
			bool first = true;

			if (orderByClauses.Count > 0)
			{
				sql.Append(" ORDER BY ");

				foreach (Tuple<ClauseItem, ListSortDirection> orderByClause in orderByClauses)
				{
					if (first)
						first = false;
					else
						sql.Append(", ");

					LinkedListStringBuilder orderBySQL = orderByClause.Item1.BuildClause(dialect, orderByParameters);

					sql.Append(orderBySQL);
					sql.Append(" ");
					sql.Append(orderByClause.Item2 == ListSortDirection.Ascending ? "ASC" : "DESC");
				}
			}

			if (skipCount != null || takeCount != null)
				dialect.SkipTakeClause(sql, skipCount, takeCount);

			IEnumerable<Parameter> parameters = selectParameters.Concat(whereParameters).Concat(orderByParameters);

			if (joinSQL != null)
				parameters = parameters.Concat(joinParameters);

			return new QueryInfo(sql.ToString(), parameters, selectConversions, isSingleColumnSelect);
		}

		private void SetSelectSQL(object obj, params ClauseGetter[] clauseGetters)
		{
			selectParameters = new ParameterCollection(nameProvider);
			selectConversions = new Dictionary<string, Type>();

			if (clauseGetters.Contains(obj))
			{
				isSingleColumnSelect = false;

				selectSQL = "*";
			}
			else if (obj is ClauseItem)
			{
				isSingleColumnSelect = true;

				ClauseItem item = CheckForConversion(new Tuple<string, ClauseItem>("Column", (ClauseItem) obj), selectConversions);

				selectSQL = item.BuildClause(dialect, selectParameters);
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

					selectSQL.Append(item.BuildClause(dialect, selectParameters));
					selectSQL.Append(" AS [" + property.Item1 + "]");
				}
			}
		}

		private static IEnumerable<Tuple<string, ClauseItem>> GetClauseItems(object obj)
		{
			return obj.GetType().GetProperties().Select(p => new Tuple<string, ClauseItem>(p.Name, (ClauseItem) p.GetValue(obj, null)));
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
