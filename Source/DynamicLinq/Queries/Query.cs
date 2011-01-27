using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DynamicLinq.ClauseItems;
using DynamicLinq.Dialects;

namespace DynamicLinq.Queries
{
	public class Query : IEnumerable<object>
	{
		private readonly IDialect dialect;
		private readonly string tableName;
		private readonly ClauseGetter clauseGetter;
		private readonly IQueryBuilder queryBuilder;

		internal Query(IDialect dialect, string tableName)
		{
			this.dialect = dialect;
			this.tableName = tableName;
			clauseGetter = new ClauseGetter(tableName);
			queryBuilder = dialect.GetQueryBuilder(tableName);
		}

		public ExtendedQuery Select(Func<dynamic, object> selector)
		{
			object obj = selector(clauseGetter);

			SetSelector(obj, clauseGetter);

			return new ExtendedQuery(dialect, queryBuilder);
		}

		public Query Where(Func<dynamic, object> predicate)
		{
			object obj = predicate(clauseGetter);

			if (!(obj is ClauseItem))
				throw new ArgumentException("Invalid predicate");

			ClauseItem clauseItem = (ClauseItem) obj;

			queryBuilder.AddWhereClause(clauseItem);

			return this;
		}

		public ExtendedQuery Join(Query inner, Func<dynamic, object> outerKeySelector, Func<dynamic, object> innerKeySelector, Func<dynamic, dynamic, object> resultSelector)
		{
			object outerKey = outerKeySelector(clauseGetter);
			object innerKey = innerKeySelector(inner.clauseGetter);

			ClauseItem joinClause;

			if (outerKey == clauseGetter || innerKey == inner.clauseGetter)
			{
				throw new ArgumentException("Invalid key selector");
			}
			else if (outerKey is ClauseItem && innerKey is ClauseItem)
			{
				joinClause = new BinaryOperation(SimpleOperator.Equal, (ClauseItem)outerKey, (ClauseItem)innerKey);
			}
			else
			{
				ClauseItem[] outerKeyProperties = GetClauseItems(outerKey).Select(property => property.Item2).ToArray();
				ClauseItem[] innerKeyProperties = GetClauseItems(innerKey).Select(property => property.Item2).ToArray();

				if (outerKeyProperties.Length == 0 || innerKeyProperties.Length == 0)
					throw new ArgumentException("Invalid key selector");

				if (outerKeyProperties.Length != innerKeyProperties.Length)
					throw new ArgumentException("Unequal number of selectors");

				joinClause = new BinaryOperation(SimpleOperator.Equal, outerKeyProperties[0], innerKeyProperties[0]);

				for (int i = 1; i < outerKeyProperties.Length; ++i)
				{
					ClauseItem additionalClause = new BinaryOperation(SimpleOperator.Equal, outerKeyProperties[i], innerKeyProperties[i]);
					joinClause = new BinaryOperation(SimpleOperator.And, joinClause, additionalClause);
				}
			}

			queryBuilder.WithJoin(joinClause, inner.tableName);

			object obj = resultSelector(clauseGetter, inner.clauseGetter);

			SetSelector(obj, clauseGetter, inner.clauseGetter);

			return new ExtendedQuery(dialect, queryBuilder);
		}

		public Query OrderBy(Func<dynamic, object> keySelector)
		{
			AddOrderBy(keySelector, ListSortDirection.Ascending);

			return this;
		}

		public Query OrderByDescending(Func<dynamic, object> keySelector)
		{
			AddOrderBy(keySelector, ListSortDirection.Descending);

			return this;
		}

		public int Count()
		{
			return (int) LongCount();
		}

		public long LongCount()
		{
			queryBuilder.SetCountSelector();

			using (IEnumerator<object> enumerator = ((IEnumerable<object>) this).GetEnumerator())
			{
				enumerator.MoveNext();

				return Convert.ToInt64(enumerator.Current);
			}
		}

		public ExtendedQuery Skip(int count)
		{
			queryBuilder.AddSkip(count);

			return new ExtendedQuery(dialect, queryBuilder);
		}

		public IEnumerable<dynamic> Take(int count)
		{
			queryBuilder.SetTake(count);

			return this;
		}

		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			return new QueryEnumerator(dialect, queryBuilder.Build());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>) this).GetEnumerator();
		}

		private void AddOrderBy(Func<dynamic, object> keySelector, ListSortDirection listSortDirection)
		{
			object obj = keySelector(clauseGetter);

			if (!(obj is ClauseItem))
				throw new ArgumentException("Invalid key selector");

			ClauseItem clauseItem = (ClauseItem) obj;

			queryBuilder.AddOrderBy(clauseItem, listSortDirection);
		}

		private void SetSelector(object obj, params ClauseGetter[] clauseGetters)
		{
			IEnumerable<Tuple<string, ClauseItem>> selections;
			SelectType selectType;
			IDictionary<string, Type> conversions = new Dictionary<string, Type>();

			if (clauseGetters.Contains(obj))
			{
				selections = null;
				selectType = SelectType.All;
			}
			else if (obj is ClauseItem)
			{
				const string selectionName = "Selection";
				selections = new[] {new Tuple<string, ClauseItem>(selectionName, CheckForConversion(new Tuple<string, ClauseItem>(selectionName, (ClauseItem) obj), conversions))};
				selectType = SelectType.Single;
			}
			else
			{
				selections = GetClauseItems(obj).Select(property => new Tuple<string, ClauseItem>(property.Item1, CheckForConversion(property, conversions)));
				selectType = SelectType.Multiple;
			}

			queryBuilder.WithSelector(selections, selectType, conversions);
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

		private static IEnumerable<Tuple<string, ClauseItem>> GetClauseItems(object obj)
		{
			return obj.GetType().GetProperties().Select(property => new Tuple<string, ClauseItem>(property.Name, (ClauseItem) property.GetValue(obj, null)));
		}
	}
}
