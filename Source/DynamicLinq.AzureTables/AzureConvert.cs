using System;
using System.Data.Services.Client;
using System.Linq;
using System.Reflection;

namespace DynamicLinq.AzureTables
{
	internal static class AzureConvert
	{
		private static readonly Type clientConvert = typeof (DataServiceContext).Assembly.GetTypes().Where(type => type.Name == "ClientConvert").First();

		internal static string KeyPrimitiveToString(object value)
		{
			object[] args = new[] {value, default(string)};

			bool success = (bool) GetMethod("TryKeyPrimitiveToString").Invoke(null, args);

			if (!success)
				throw new InvalidOperationException("Could not convert value to string");

			return (string) args[1];
		}

		internal static string GetEdmType(Type type)
		{
			return (string) GetMethod("GetEdmType").Invoke(null, new[] {type});
		}

		internal static string ToString(object value)
		{
			return (string) GetMethod("ToString").Invoke(null, new[] {value, false});
		}

		internal static object ParseValue(string edmType, string value)
		{
			object[] args = new object[] {edmType, default(Type)};

			bool success = (bool) GetMethod("ToNamedType").Invoke(null, args);

			if (!success)
				throw new InvalidOperationException("Could not parse value from xml");

			return GetMethod("ChangeType").Invoke(null, new[] {value, args[1]});
		}

		private static MethodInfo GetMethod(string name)
		{
			return clientConvert.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static);
		}
	}
}