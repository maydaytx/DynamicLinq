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

		public static void StoreLocal(this ILGenerator gen, LocalBuilder local)
		{
			switch (local.LocalIndex)
			{
				case 0:
					gen.Emit(OpCodes.Stloc_0); break;
				case 1:
					gen.Emit(OpCodes.Stloc_1); break;
				case 2:
					gen.Emit(OpCodes.Stloc_2); break;
				case 3:
					gen.Emit(OpCodes.Stloc_3); break;
				default:
					gen.Emit(OpCodes.Stloc_S, local.LocalIndex); break;
			}
		}

		public static void LoadLocal(this ILGenerator gen, LocalBuilder local)
		{
			OpCode ldLocOpCode;

			switch (local.LocalIndex)
			{
				case 0:
					ldLocOpCode = OpCodes.Ldloc_0; break;
				case 1:
					ldLocOpCode = OpCodes.Ldloc_1; break;
				case 2:
					ldLocOpCode = OpCodes.Ldloc_2; break;
				case 3:
					ldLocOpCode = OpCodes.Ldloc_3; break;
				default:
					ldLocOpCode = OpCodes.Ldloc_S; break;
			}

			if (ldLocOpCode == OpCodes.Ldloc_S)
				gen.Emit(OpCodes.Ldloc_S, local.LocalIndex);
			else gen.Emit(ldLocOpCode);
		}

		public static void Call<T>(this ILGenerator gen, string methodName, BindingFlags bindingFlags, params Type[] types)
		{
			gen.EmitCall(OpCodes.Call, typeof (T).GetMethod(methodName, bindingFlags, null, types, null), null);
		}

		public static Label BreakShortForm(this ILGenerator gen)
		{
			Label label = gen.DefineLabel();

			gen.Emit(OpCodes.Br_S, label);

			return label;
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