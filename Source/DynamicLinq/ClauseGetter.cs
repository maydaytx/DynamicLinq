using System.Dynamic;
using DynamicLinq.ClauseItems;

namespace DynamicLinq
{
	public class ClauseGetter : DynamicObject
	{
		internal ClauseGetter() { }

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			string name = binder.Name;

			result = new Column(name);

			return true;
		}
	}
}