using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DynamicLinq.ClauseItems;
using DynamicLinq.Queries;

namespace DynamicLinq.AzureTables
{
	internal class AzureTablesQueryBuilder : IQueryBuilder
	{
		private readonly string tableName;
		
		private ClauseItem whereClause;
		private int? takeCount;
		private bool isCountSelector;

		public AzureTablesQueryBuilder(string tableName)
		{
			this.tableName = tableName;
		}

		public void AddWhereClause(ClauseItem clauseItem)
		{
			if (ReferenceEquals(whereClause, null))
				whereClause = clauseItem;
			else
				whereClause = whereClause && clauseItem;
		}

		public void WithSelector(IEnumerable<Tuple<string, ClauseItem>> selections, SelectType selectType, IDictionary<string, Type> conversions)
		{
			if (selectType != SelectType.All)
				throw new NotSupportedException("Azure Tables only supports selecting all properties in a table");
		}

		public void WithJoin(ClauseItem joinClause, string innerTableName)
		{
			throw new NotSupportedException("Join() is not supported for Azure Tables");
		}

		public void AddOrderBy(ClauseItem clauseItem, ListSortDirection sortDirection)
		{
			throw new NotSupportedException("OrderBy() is not supported on Azure Tables");
		}

		public void AddSkip(int count)
		{
			throw new NotSupportedException("Skip() is not supported on Azure Tables");
		}

		public void SetTake(int count)
		{
			takeCount = count;
		}

		public void SetCountSelector()
		{
			isCountSelector = true;
		}

		public QueryInfo Build()
		{
			StringBuilder query = new StringBuilder(tableName + "()");

			if (isCountSelector)
				query.Append("/$count");

			bool first = true;

			if (!ReferenceEquals(whereClause, null))
			{
				query.Append("?");
				first = false;

				query.Append("$filter=");
				query.Append(whereClause.BuildClause(new AzureTablesDialect(), null));
			}

			if (takeCount != null)
			{
				if (first)
				{
					query.Append("?");
				}
				else
				{
					query.Append("&");
				}

				query.Append("$top=");
				query.Append(takeCount.Value);
			}

			return new QueryInfo(query.ToString(), Enumerable.Empty<Parameter>(), new Dictionary<string, Type>(), false);
		}
	}
}