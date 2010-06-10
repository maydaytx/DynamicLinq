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

		internal override AwesomeStringBuilder BuildClause(IList<Tuple<string, object>> parameters)
		{
			if (@object is string)
			{
				string parameterName = "@p" + parameters.Count;

				parameters.Add(new Tuple<string, object>(parameterName, @object));

				return new AwesomeStringBuilder(parameterName);
			}
			else if (@object is char || @object is DateTime)
			{
				return new AwesomeStringBuilder("'" + @object + "'");
			}
			else
			{
				return new AwesomeStringBuilder(@object.ToString());
			}
		}
	}
}
