using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;

namespace DynamicLinq.Collections
{
	internal class DynamicBag : DynamicObject, ISerializable
	{
		private readonly IDictionary<string, object> values = new Dictionary<string, object>();

		internal DynamicBag() { }

		private DynamicBag(SerializationInfo info, StreamingContext context)
		{
			foreach (SerializationEntry serializationEntry in info)
				values.Add(serializationEntry.Name, serializationEntry.Value);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (values.ContainsKey(binder.Name))
			{
				result = values[binder.Name];
				return true;
			}

			result = null;
			return false;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (values.ContainsKey(binder.Name))
				values[binder.Name] = value;
			else
				values.Add(binder.Name, value);

			return true;
		}

		public void SetValue(string name, object value)
		{
			if (values.ContainsKey(name))
				values[name] = value;
			else
				values.Add(name, value);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			foreach (KeyValuePair<string, object> value in values)
				info.AddValue(value.Key, value.Value);
		}
	}
}
