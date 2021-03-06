﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;

namespace DynamicLinq.Collections
{
	public class DynamicBag : DynamicObject, ISerializable
	{
		private readonly IDictionary<string, object> values = new Dictionary<string, object>();

		public DynamicBag() { }

		private DynamicBag(SerializationInfo info, StreamingContext context)
		{
			foreach (SerializationEntry serializationEntry in info)
				values.Add(serializationEntry.Name, serializationEntry.Value);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return TryGetValue(binder.Name, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			SetValue(binder.Name, value);

			return true;
		}

		public object GetValue(string name)
		{
			return values[name];
		}

		public bool TryGetValue(string name, out object result)
		{
			if (values.ContainsKey(name))
			{
				result = values[name];
				return true;
			}

			result = null;
			return false;
		}

		public void SetValue(string name, object value)
		{
			if (values.ContainsKey(name))
				values[name] = value;
			else
				values.Add(name, value);
		}

		public IEnumerable<Tuple<string, object>> Values
		{
			get { return values.Select(value => new Tuple<string, object>(value.Key, value.Value)); }
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			foreach (KeyValuePair<string, object> value in values)
				info.AddValue(value.Key, value.Value);
		}
	}
}
