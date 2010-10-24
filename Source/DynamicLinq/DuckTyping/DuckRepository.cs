using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Threading;

namespace DynamicLinq.DuckTyping
{
	public static class DuckRepository
	{
		internal const string AssemblyName = "Ducks";
		internal const string ClassPrefix = "Duck";

		private static readonly AssemblyBuilder assemblyBuilder;
		private static readonly ModuleBuilder moduleBuilder;

		private static readonly IDictionary<string, Type> generatedDuckTypes;
		private static int count;

		static DuckRepository()
		{
			assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.Run);
			moduleBuilder = assemblyBuilder.DefineDynamicModule(AssemblyName);
			generatedDuckTypes = new Dictionary<string, Type>();
			count = 0;
		}

		public static void ResetCache()
		{
			lock (generatedDuckTypes)
			{
				generatedDuckTypes.Clear();
			}
		}

		internal static object CreateDuck(string typeId, IEnumerable<Tuple<string, Type, object>> properties)
		{
			Type type = null;

			lock (generatedDuckTypes)
			{
				if (generatedDuckTypes.ContainsKey(typeId))
					type = generatedDuckTypes[typeId];
			}

			if (type == null)
				type = GenerateDuckType(Enumerable.Select(properties, p => new Tuple<string, Type>(p.Item1, p.Item2)));

			return CreateDuck(type, properties);
		}

		internal static object CreateDuck(Type duckType, IEnumerable<Tuple<string, Type, object>> properties)
		{
			Duck duck = (Duck) Activator.CreateInstance(duckType);

			foreach (Tuple<string, Type, object> property in properties)
			{
				duck.EnsureProperty(property.Item1, property.Item2);
				duck.SetPropertyValue(property.Item1, property.Item3);
			}

			return duck;
		}

		internal static Type GenerateDuckType(string typeId)
		{
			Type type;

			lock (generatedDuckTypes)
			{
				if (generatedDuckTypes.ContainsKey(typeId))
				{
					type = generatedDuckTypes[typeId];
				}
				else
				{
					string[] propertiesAndTypes = typeId.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

					Tuple<string, Type>[] properties = new Tuple<string, Type>[propertiesAndTypes.Length/2];

					for (int i = 0; i < propertiesAndTypes.Length; i += 2)
						properties[i/2] = new Tuple<string, Type>(propertiesAndTypes[i], Type.GetType(propertiesAndTypes[i + 1]));

					type = GenerateDuckType(typeId, properties);
				}
			}

			return type;
		}

		internal static Type GenerateDuckType(IEnumerable<Tuple<string, Type>> properties)
		{
			LinkedListStringBuilder sb = new LinkedListStringBuilder();

			string typeId = Enumerable.Aggregate(Enumerable.OrderBy(properties, p => p.Item1), sb, (current, property) => current + (property.Item1 + '|' + property.Item2.AssemblyQualifiedName + '|')).ToString();

			return GenerateDuckType(typeId, properties);
		}

		internal static Type GenerateDuckType(string typeId, IEnumerable<Tuple<string, Type>> properties)
		{
			Type type;

			lock (generatedDuckTypes)
			{
				if (generatedDuckTypes.ContainsKey(typeId))
				{
					type = generatedDuckTypes[typeId];
				}
				else
				{
					TypeBuilder typeBuilder = moduleBuilder.DefineType(ClassPrefix + count, TypeAttributes.Public | TypeAttributes.Class, typeof(Duck), new[] { typeof(ISerializable) });

					typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (SerializableAttribute).GetConstructor(new Type[0]), new object[0]));

					DefineGetTypeId(typeBuilder, typeId);

					DefineSerializationMethod(typeBuilder);

					foreach (Tuple<string, Type> property in properties)
						DefineProperty(typeBuilder, property.Item1, property.Item2);

					type = typeBuilder.CreateType();

					++count;

					generatedDuckTypes.Add(typeId, type);
				}
			}

			return type;
		}

		private static void DefineGetTypeId(TypeBuilder tb, string typeId)
		{
			MethodInfo method = typeof (Duck).GetMethod("GetTypeId", BindingFlags.NonPublic | BindingFlags.Instance);

			MethodBuilder mb = DefineMethodSignature(tb, "GetTypeId", method.ReturnType, method.GetParameters());

			ILGenerator gen = mb.GetILGenerator();

			LocalBuilder init = gen.DeclareLocal(typeof (string));

			gen.Nop();
			gen.LoadString(typeId);
			gen.StoreLocal(init);
			Label label = gen.BreakShortForm();
			gen.MarkLabel(label);
			gen.LoadLocal(init);
			gen.Return();

			tb.DefineMethodOverride(mb, method);
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