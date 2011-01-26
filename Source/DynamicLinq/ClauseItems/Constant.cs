using DynamicLinq.Collections;
using DynamicLinq.Dialects;

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

		internal override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			return dialect.Constant(@object, parameters);
		}
	}
}
