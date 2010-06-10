using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Threading;

namespace DynamicLinq
{
	internal static class DuckRepository
	{
		internal const string AssemblyName = "DuckProxy";
		internal const string ClassPrefix = "Duck";

		private static readonly AssemblyBuilder assemblyBuilder;
		private static readonly ModuleBuilder moduleBuilder;

		private static readonly IDictionary<string, Type> generatedDuckTypes;

		static DuckRepository()
		{
			assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.Run);
			moduleBuilder = assemblyBuilder.DefineDynamicModule(AssemblyName);
			generatedDuckTypes = new Dictionary<string, Type>();
		}

		internal static object CreateDuck(Type duckType, IDictionary<string, Type> properties, IEnumerable<Tuple<string, object>> row)
		{
			Duck duck = (Duck) Activator.CreateInstance(duckType);

			foreach (Tuple<string, object> property in row)
			{
				duck.EnsureProperty(property.Item1, properties[property.Item1]);
				duck.SetPropertyValue(property.Item1, property.Item2);
			}

			return duck;
		}

		internal static Type GenerateDuckType(IEnumerable<Tuple<string, Type>> properties)
		{
			Type type;

			lock (generatedDuckTypes)
			{
				AwesomeStringBuilder sb = new AwesomeStringBuilder();

				sb = Enumerable.Aggregate(Enumerable.OrderBy(properties, p => p.Item1), sb, (current, property) => current + ("[" + property.Item1 + "][" + property.Item2.FullName + "]"));

				string id = sb.ToString();

				if (generatedDuckTypes.ContainsKey(id))
				{
					type = generatedDuckTypes[id];
				}
				else
				{
					TypeBuilder typeBuilder = moduleBuilder.DefineType(ClassPrefix + generatedDuckTypes.Count, TypeAttributes.Public | TypeAttributes.Class, typeof (Duck), new[] {typeof (ISerializable)});

					typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(SerializableAttribute).GetConstructor(new Type[0]), new object[0]));

					DefineSerializationMethod(typeBuilder);

					foreach (Tuple<string, Type> property in properties)
						DefineProperty(typeBuilder, property.Item1, property.Item2);

					type = typeBuilder.CreateType();

					generatedDuckTypes.Add(id, type);
				}
			}

			return type;
		}

		private static void DefineSerializationMethod(TypeBuilder tb)
		{
			MethodInfo method = typeof (ISerializable).GetMethod("GetObjectData");

			MethodBuilder mb = DefineMethodSignature(tb, method.Name, method.ReturnType, method.GetParameters());

			ILGenerator gen = mb.GetILGenerator();

			gen.Nop();
			gen.LoadThis();
			gen.LoadArgument(0);
			gen.Call<Duck>("GetObjectData", BindingFlags.NonPublic | BindingFlags.Instance, typeof (SerializationInfo));
			gen.Nop();
			gen.Return();

			tb.DefineMethodOverride(mb, method);
		}

		private static void DefineProperty(TypeBuilder tb, string propertyName, Type propertyType)
		{
			PropertyBuilder pb = tb.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);

			pb.SetGetMethod(DefinePropertyGetMethod(tb, propertyName, propertyType));

			pb.SetSetMethod(DefinePropertySetMethod(tb, propertyName, propertyType));
		}

		private static MethodBuilder DefinePropertyGetMethod(TypeBuilder tb, string propertyName, Type propertyType)
		{
			MethodBuilder mb = DefineMethodSignature(tb, "get_" + propertyName, propertyType, new ParameterInfo[0]);

			ILGenerator gen = mb.GetILGenerator();

			CallEnsureProperty(propertyName, propertyType, gen);

			gen.LoadThis();
			gen.LoadString(propertyName);
			gen.Call<Duck>("GetPropertyValue", BindingFlags.NonPublic | BindingFlags.Instance, typeof (string));
			gen.UnboxOrCast(propertyType);

			gen.Return();

			return mb;
		}

		private static MethodBuilder DefinePropertySetMethod(TypeBuilder tb, string propertyName, Type propertyType)
		{
			MethodBuilder mb = DefineMethodSignature(tb, "set_" + propertyName, typeof (void), new ParameterInfo[] { new NewParameterInfo("value", propertyType) });

			ILGenerator gen = mb.GetILGenerator();

			CallEnsureProperty(propertyName, propertyType, gen);

			gen.LoadThis();
			gen.LoadString(propertyName);
			gen.LoadArgument(0);
			gen.BoxIfPrimitive(propertyType);
			gen.Call<Duck>("SetPropertyValue", BindingFlags.NonPublic | BindingFlags.Instance, typeof (string), typeof (object));

			gen.Return();

			return mb;
		}

		private static void CallEnsureProperty(string propertyName, Type propertyType, ILGenerator gen)
		{
			gen.LoadThis();
			gen.LoadString(propertyName);
			gen.LoadType(propertyType);
			gen.Call<Duck>("EnsureProperty", BindingFlags.NonPublic | BindingFlags.Instance, typeof(string), typeof(Type));
		}

		private static MethodBuilder DefineMethodSignature(TypeBuilder tb, string methodName, Type returnType, IEnumerable<ParameterInfo> parameters)
		{
			Type[] parameterTypes = Enumerable.Select(parameters, p => p.ParameterType).ToArray();

			MethodBuilder mb = tb.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Virtual, returnType, parameterTypes);

			int i = 0;

			foreach (ParameterInfo parameter in parameters)
				mb.DefineParameter(++i, ParameterAttributes.None, parameter.Name);

			return mb;
		}

		private class NewParameterInfo : ParameterInfo
		{
			public NewParameterInfo(string name, Type parameterType)
			{
				NameImpl = name;
				ClassImpl = parameterType;
				AttrsImpl = ParameterAttributes.None;
			}
		}
	}
}