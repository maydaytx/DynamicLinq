using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DynamicLinq.ClauseItems;
using DynamicLinq.Collections;
using DynamicLinq.Dialects;

namespace DynamicLinq.Queries
{
	internal class SQLQueryBuilder : IQueryBuilder
	{
		private readonly SQLDialect dialect;
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

		internal SQLQueryBuilder(SQLDialect dialect, string tableName)
		{
			this.dialect = dialect;
			this.tableName = tableName;

			nameProvider = new ParameterNameProvider(dialect);
			orderByClauses = new List<Tuple<ClauseItem, ListSortDirection>>();
		}

		void IQueryBuilder.AddWhereClause(ClauseItem clauseItem)
		{
			if (ReferenceEquals(whereClause, null))
				whereClause = clauseItem;
			else
				whereClause = new BinaryOperation(SimpleOperator.And, whereClause, clauseItem);
		}

		void IQueryBuilder.WithSelector(IEnumerable<Tuple<string, ClauseItem>> selections, SelectType selectType, IDictionary<string, Type> conversions)
		{
			selectParameters = new ParameterCollection(nameProvider);
			selectConversions = conversions;

			switch (selectType)
			{
				case SelectType.All:
					isSingleColumnSelect = false;
					selectSQL = "*";
					break;
				case SelectType.Single:
					isSingleColumnSelect = true;
					selectSQL = selections.First().Item2.BuildClause(dialect, selectParameters);
					break;
				case SelectType.Multiple:
					isSingleColumnSelect = false;

					selectSQL = new LinkedListStringBuilder();

					bool notFirst = false;

					foreach (Tuple<string, ClauseItem> property in selections)
					{
						if (notFirst)
							selectSQL.Append(", ");
						else
							notFirst = true;

						selectSQL.Append(property.Item2.BuildClause(dialect, selectParameters));
						selectSQL.Append(" AS [" + property.Item1 + "]");
					}
					break;
			}
		}

		void IQueryBuilder.WithJoin(ClauseItem joinClause, string innerTableName)
		{
			joinTableName = innerTableName;
			joinParameters = new ParameterCollection(nameProvider);
			
			joinSQL = joinClause.BuildClause(dialect, joinParameters);
		}

		void IQueryBuilder.AddOrderBy(ClauseItem clauseItem, ListSortDirection sortDirection)
		{
			orderByClauses.Add(new Tuple<ClauseItem, ListSortDirection>(clauseItem, sortDirection));
		}

		void IQueryBuilder.AddSkip(int count)
		{
			if (skipCount != null)
				skipCount += count;
			else
				skipCount = count;
		}

		void IQueryBuilder.SetTake(int count)
		{
			takeCount = count;
		}

		void IQueryBuilder.SetCountSelector()
		{
			isCountSelector = true;
		}

		QueryInfo IQueryBuilder.Build()
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
	}
}
