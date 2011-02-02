using System.Dynamic;

namespace DynamicLinq
{
	public class NameGetter : DynamicObject
	{
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = binder.Name;

			return true;
		}
	}
}