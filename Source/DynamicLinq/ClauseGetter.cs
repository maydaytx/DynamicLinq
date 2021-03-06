﻿using System.Dynamic;
using DynamicLinq.ClauseItems;

namespace DynamicLinq
{
	public class ClauseGetter : DynamicObject
	{
		private readonly string tableName;

		internal ClauseGetter(string tableName)
		{
			this.tableName = tableName;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			string name = binder.Name;

			result = new Column(tableName, name);

			return true;
		}
	}
}