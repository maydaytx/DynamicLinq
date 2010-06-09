using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DynamicLinq
{
	[Serializable]
	public abstract class Duck
	{
		private readonly Dictionary<string, object> properties;

		protected Duck()
		{
			properties = new Dictionary<string, object>();
		}

		protected object GetPropertyValue(string key)
		{
			object value = properties[key];

			return value;
		}

		protected internal void SetPropertyValue(string key, object value)
		{
			properties[key] = value;
		}

		protected internal void EnsureProperty(string key, Type propertyType)
		{
			if (!properties.ContainsKey(key))
				properties.Add(key, propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null);
		}

		protected void GetObjectData(SerializationInfo info)
		{
			foreach (KeyValuePair<string, object> property in properties)
				info.AddValue(property.Key, property.Value);
		}
	}
}
