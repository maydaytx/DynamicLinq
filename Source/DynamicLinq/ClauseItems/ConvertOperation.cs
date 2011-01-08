using System;
using System.Collections.Generic;
using DynamicLinq.Collections;
using DynamicLinq.Dialect;

namespace DynamicLinq.ClauseItems
{
	public class ConvertOperation : ClauseItem
	{
		private readonly ClauseItem item;
		private readonly Type type;

		internal ClauseItem Item
		{
			get { return item; }
		}

		internal Type Type
		{
			get { return type; }
		}

		internal ConvertOperation(ClauseItem item, Type type)
		{
			this.item = item;
			this.type = type;
		}

		internal override LinkedListStringBuilder BuildClause(SQLDialect dialect, IList<Tuple<string, object>> parameters, ParameterNameProvider nameProvider)
		{
			throw new NotSupportedException("Conversions should only occur in selects");
		}
	}
}
