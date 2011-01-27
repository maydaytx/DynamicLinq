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

		public override bool ShouldParenthesize
		{
			get { return false; }
		}

		public override LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters)
		{
			return dialect.Constant(@object, parameters);
		}

		public override string ToString()
		{
			return @object.ToString();
		}
	}
}
