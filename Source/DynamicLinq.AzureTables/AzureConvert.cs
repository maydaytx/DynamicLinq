using System;
using System.Data.Services.Client;
using System.Linq;
using System.Reflection;

namespace DynamicLinq.AzureTables
{
	internal static class AzureConvert
	{
		private delegate bool TryKeyPrimitiveToString(object value, out string result);
		private delegate bool ToNamedType(string typeName, out Type type);

		private static readonly TryKeyPrimitiveToString tryKeyPrimitiveToString;
		private static readonly Func<object, bool, string> toString;
		private static readonly ToNamedType toNamedType;
		private static readonly Func<string, Type, object> changeType;

		static AzureConvert()
		{
			const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;

			Type clientConvert = typeof (DataServiceContext).Assembly.GetTypes().Where(type => type.Name == "ClientConvert").First();

			tryKeyPrimitiveToString = (TryKeyPrimitiveToString) Delegate.CreateDelegate(typeof (TryKeyPrimitiveToString), clientConvert.GetMethod("TryKeyPrimitiveToString", flags));
			GetEdmType = (Func<Type, string>) Delegate.CreateDelegate(typeof (Func<Type, string>), clientConvert.GetMethod("GetEdmType", flags));
			toString = (Func<object, bool, string>) Delegate.CreateDelegate(typeof (Func<object, bool, string>), clientConvert.GetMethod("ToString", flags));
			toNamedType = (ToNamedType) Delegate.CreateDelegate(typeof (ToNamedType), clientConvert.GetMethod("ToNamedType", flags));
			changeType = (Func<string, Type, object>) Delegate.CreateDelegate(typeof (Func<string, Type, object>), clientConvert.GetMethod("ChangeType", flags));
		}

		internal static string KeyPrimitiveToString(object value)
		{
			string result;

			if (tryKeyPrimitiveToString(value, out result))
			{
				return result;
			}
			else
			{
				throw new InvalidOperationException("Could not convert value to string");
			}
		}

		internal static readonly Func<Type, string> GetEdmType;

		internal static string ToString(object value)
		{
			return toString(value, false);
		}

		internal static object ParseValue(string edmType, string value)
		{
			Type type;

			if (toNamedType(edmType, out type))
			{
				return changeType(value, type);
			}
			else
			{
				throw new InvalidOperationException("Could not parse value from xml");
			}
		}
	}
}