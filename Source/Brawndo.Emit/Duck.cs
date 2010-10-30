using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Brawndo.Emit
{
	[Serializable]
	public abstract class Duck
	{
		private const string TypeIdSerializationName = "DD04B332750511DFA9CA8B2FE0D7208";

		private readonly Dictionary<string, object> properties;

		protected Duck()
		{
			properties = new Dictionary<string, object>();
		}

		protected abstract string GetTypeId();

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
			info.AddValue(TypeIdSerializationName, GetTypeId());

			foreach (KeyValuePair<string, object> property in properties)
				info.AddValue(property.Key, property.Value);
		}

		private static object Deserialize(SerializationInfo info)
		{
			string typeId = info.GetString(TypeIdSerializationName);

			Type duckType = DuckRepository.GenerateDuckType(typeId);

			IList<Tuple<string, Type, object>> serializationInfo = new List<Tuple<string, Type, object>>();

			foreach (SerializationEntry serializationEntry in info)
				if (serializationEntry.Name != TypeIdSerializationName)
					serializationInfo.Add(new Tuple<string, Type, object>(serializationEntry.Name, serializationEntry.ObjectType, serializationEntry.Value));

			return DuckRepository.CreateDuck(duckType, serializationInfo);
		}

		public static void Configure(IFormatter formatter)
		{
			BinderAndSurrogate binderAndSurrogate = new BinderAndSurrogate();

			SurrogateSelector surrogateSelector = new SurrogateSelector();
			surrogateSelector.AddSurrogate(BinderAndSurrogate.DummyDuckType, new StreamingContext(StreamingContextStates.All), binderAndSurrogate);

			if (formatter.SurrogateSelector != null)
				formatter.SurrogateSelector.ChainSelector(surrogateSelector);
			else
				formatter.SurrogateSelector = surrogateSelector;

			formatter.Binder = binderAndSurrogate;
		}

		private class BinderAndSurrogate : SerializationBinder, ISerializationSurrogate
		{
			public static readonly Type DummyDuckType = DuckRepository.GenerateDuckType(new Tuple<string, Type>[0]);

			public override Type BindToType(string assemblyName, string typeName)
			{
				if (assemblyName.StartsWith(DuckRepository.AssemblyName) && typeName.StartsWith(DuckRepository.ClassPrefix))
					return DummyDuckType;
				else
					return Type.GetType(typeName + ", " + assemblyName);
			}

			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) { }

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
			{
				return Deserialize(info);
			}
		}
	}
}
