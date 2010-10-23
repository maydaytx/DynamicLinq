using System;
using System.Collections.Generic;

namespace DynamicLinq.ClauseItems
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

		internal override LinkedListStringBuilder BuildClause(IList<Tuple<string, object>> parameters)
		{
			if (@object is string)
			{
				string parameterName = "@p" + parameters.Count;

				parameters.Add(new Tuple<string, object>(parameterName, @object));

				return new LinkedListStringBuilder(parameterName);
			}
			else if (@object is char)
			{
				return new LinkedListStringBuilder("'" + @object + "'");
			}
			else if (@object is DateTime)
			{
				return new LinkedListStringBuilder("'" + ((DateTime) @object).ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "'");
			}
			else
			{
				return new LinkedListStringBuilder(@object.ToString());
			}
		}
	}
}
