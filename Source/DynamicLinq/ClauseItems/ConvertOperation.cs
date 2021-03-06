﻿using System;
using DynamicLinq.Collections;
using DynamicLinq.Dialects;

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

		public override bool ShouldParenthesize
		{
			get { throw new NotSupportedException("Conversions should only occur in selects"); }
		}

		public override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			throw new NotSupportedException("Conversions should only occur in selects");
		}

		public override string ToString()
		{
			return "Convert(" + item.ToString() + ", " + type + ")";
		}
	}
}
