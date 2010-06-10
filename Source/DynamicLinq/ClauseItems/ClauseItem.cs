﻿using System;
using System.Collections.Generic;

namespace DynamicLinq.ClauseItems
{
	public abstract class ClauseItem
	{
		internal ClauseItem() { }

		internal abstract AwesomeStringBuilder BuildClause(IList<Tuple<string, object>> parameters);

		//public override string ToString()
		//{
		//    return BuildClause(new List<Tuple<string, object>>()).ToString();
		//}

		#region implicit conversions

		public static implicit operator ClauseItem(bool @bool)
		{
			return new Constant(@bool);
		}

		public static implicit operator ClauseItem(byte @byte)
		{
			return new Constant(@byte);
		}

		public static implicit operator ClauseItem(sbyte @sbyte)
		{
			return new Constant(@sbyte);
		}

		public static implicit operator ClauseItem(char @char)
		{
			return new Constant(@char);
		}

		public static implicit operator ClauseItem(decimal @decimal)
		{
			return new Constant(@decimal);
		}

		public static implicit operator ClauseItem(double @double)
		{
			return new Constant(@double);
		}

		public static implicit operator ClauseItem(float @float)
		{
			return new Constant(@float);
		}

		public static implicit operator ClauseItem(int @int)
		{
			return new Constant(@int);
		}

		public static implicit operator ClauseItem(uint @uint)
		{
			return new Constant(@uint);
		}

		public static implicit operator ClauseItem(long @long)
		{
			return new Constant(@long);
		}

		public static implicit operator ClauseItem(ulong @ulong)
		{
			return new Constant(@ulong);
		}

		public static implicit operator ClauseItem(short @short)
		{
			return new Constant(@short);
		}

		public static implicit operator ClauseItem(ushort @ushort)
		{
			return new Constant(@ushort);
		}

		public static implicit operator ClauseItem(string @string)
		{
			return new Constant(@string);
		}

		public static implicit operator ClauseItem(DateTime dateTime)
		{
			return new Constant(dateTime);
		}

		#endregion

		#region binary operations

		//+, -, *, /, %, &, |, ^, <<, >>
		public static ClauseItem operator +(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.Add, x, y);
		}

		public static ClauseItem operator -(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.Subtract, x, y);
		}

		public static ClauseItem operator *(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.Multiply, x, y);
		}

		public static ClauseItem operator /(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.Divide, x, y);
		}

		public static ClauseItem operator %(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.Mod, x, y);
		}

		public static ClauseItem operator &(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.And, x, y);
		}

		public static ClauseItem operator |(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.Or, x, y);
		}

		public static ClauseItem operator ^(ClauseItem x, ClauseItem y)
		{
			throw new NotImplementedException();
		}

		public static ClauseItem operator <<(ClauseItem x, int y)
		{
			throw new NotImplementedException();
		}

		public static ClauseItem operator >>(ClauseItem x, int y)
		{
			throw new NotImplementedException();
		}

		//==, !=, <, >, <=, >=
		public static ClauseItem operator ==(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.Equal, x, y);
		}

		public static ClauseItem operator !=(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.NotEqual, x, y);
		}

		public static ClauseItem operator <(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.LessThan, x, y);
		}

		public static ClauseItem operator >(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.GreaterThan, x, y);
		}

		public static ClauseItem operator <=(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.LessThanOrEqual, x, y);
		}

		public static ClauseItem operator >=(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(BinaryOperator.GreaterThanOrEqual, x, y);
		}

		public ClauseItem Like(ClauseItem clauseItem)
		{
			return new BinaryOperation(BinaryOperator.Like, this, clauseItem);
		}

		#endregion

	}
}
