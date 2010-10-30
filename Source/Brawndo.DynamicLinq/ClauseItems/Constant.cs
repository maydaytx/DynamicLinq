using System;
using System.Collections.Generic;
using Brawndo.DynamicLinq.Dialect;

namespace Brawndo.DynamicLinq.ClauseItems
{
	public class Constant : ClauseItem
	{
		private readonly object @object;

		internal object Object
		{
			get { return @object; }
		}

		internal Constant(object @object)
		{
			this.@object = @object;
		}

		internal override LinkedListStringBuilder BuildClause(SQLDialect dialect, IList<Tuple<string, object>> parameters)
		{
			if (@object is string)
			{
				string parameterName = dialect.ParameterPrefix + parameters.Count;

				parameters.Add(new Tuple<string, object>(parameterName, @object));

				return parameterName;
			}
			else if (@object is char)
			{
				return "'" + @object + "'";
			}
			else if (@object is DateTime)
			{
				return "'" + ((DateTime) @object).ToString(dialect.DateTimeFormat) + "'";
			}
			else if (@object is bool)
			{
				return (bool) @object ? "1" : "0";
			}
			else
			{
				return @object.ToString();
			}
		}
	}
}
