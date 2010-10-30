using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Brawndo.Emit
{
	internal static class ILGeneratorExtensions
	{
		internal static void LoadThis(this ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldarg_0);
		}

		internal static void LoadArgument(this ILGenerator gen, int index)
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

		internal static void LoadType(this ILGenerator gen, Type type)
		{
			gen.Emit(OpCodes.Ldtoken, type);
			gen.Call<Type>("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static, typeof (RuntimeTypeHandle));
		}

		internal static void LoadString(this ILGenerator gen, string str)
		{
			gen.Emit(OpCodes.Ldstr, str);
		}

		internal static void StoreLocal(this ILGenerator gen, LocalBuilder local)
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

		internal static void LoadLocal(this ILGenerator gen, LocalBuilder local)
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

		internal static void Call<T>(this ILGenerator gen, string methodName, BindingFlags bindingFlags, params Type[] types)
		{
			gen.EmitCall(OpCodes.Call, typeof (T).GetMethod(methodName, bindingFlags, null, types, null), null);
		}

		internal static Label BreakShortForm(this ILGenerator gen)
		{
			Label label = gen.DefineLabel();

			gen.Emit(OpCodes.Br_S, label);

			return label;
		}

		internal static void Nop(this ILGenerator gen)
		{
			gen.Emit(OpCodes.Nop);
		}

		internal static void Return(this ILGenerator gen)
		{
			gen.Emit(OpCodes.Ret);
		}

		internal static void BoxIfPrimitive(this ILGenerator gen, Type type)
		{
			if (type.IsValueType)
				gen.Emit(OpCodes.Box, type);
		}

		internal static void UnboxOrCast(this ILGenerator gen, Type type)
		{
			if (type.IsValueType)
				gen.Emit(OpCodes.Unbox_Any, type);
			else
				gen.Emit(OpCodes.Castclass, type);
		}
	}
}