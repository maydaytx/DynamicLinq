using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicLinq
{
	internal static class ILGeneratorExtensions
	{
		public static void LoadThis(this ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldarg_0);
		}

		public static void LoadArgument(this ILGenerator gen, int index)
		{
			switch (index)
			{
				case 0:
					gen.Emit(OpCodes.Ldarg_1); break;
				case 1:
					gen.Emit(OpCodes.Ldarg_2); break;
				case 2:
					gen.Emit(OpCodes.Ldarg_3); break;
				default:
					gen.Emit(OpCodes.Ldarg_S, index + 1); break;
			}
		}

		public static void LoadType(this ILGenerator gen, Type type)
		{
			gen.Emit(OpCodes.Ldtoken, type);
			gen.Call<Type>("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static, typeof (RuntimeTypeHandle));
		}

		public static void LoadString(this ILGenerator gen, string str)
		{
			gen.Emit(OpCodes.Ldstr, str);
		}

		public static void Call<T>(this ILGenerator gen, string methodName, BindingFlags bindingFlags, params Type[] types)
		{
			gen.EmitCall(OpCodes.Call, typeof (T).GetMethod(methodName, bindingFlags, null, types, null), null);
		}

		public static void Nop(this ILGenerator gen)
		{
			gen.Emit(OpCodes.Nop);
		}

		public static void Return(this ILGenerator gen)
		{
			gen.Emit(OpCodes.Ret);
		}

		public static void BoxIfPrimitive(this ILGenerator gen, Type type)
		{
			if (type.IsValueType)
				gen.Emit(OpCodes.Box, type);
		}

		public static void UnboxOrCast(this ILGenerator gen, Type type)
		{
			if (type.IsValueType)
				gen.Emit(OpCodes.Unbox_Any, type);
			else
				gen.Emit(OpCodes.Castclass, type);
		}
	}
}