using System.Dynamic;

namespace DynamicLinq
{
	internal class NameGetter : DynamicObject
	{
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = binder.Name;

			return true;
		}
	}
}