using System;
using DynamicLinq.Collections;
using DynamicLinq.Dialects;

namespace DynamicLinq.ClauseItems
{
	public abstract class ClauseItem
	{
		internal ClauseItem() { }

		public abstract bool ShouldParenthesize { get; }

		public abstract LinkedListStringBuilder BuildClause(IDialect dialect, ParameterCollection parameters);

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

		#region unary operations

		//+, -, !, ~, ++, --, true, false
		public static ClauseItem operator +(ClauseItem x)
		{
			return new UnaryOperation(SimpleOperator.Positive, x);
		}

		public static ClauseItem operator -(ClauseItem x)
		{
			return new UnaryOperation(SimpleOperator.Negative, x);
		}

		public static ClauseItem operator !(ClauseItem x)
		{
			return new UnaryOperation(SimpleOperator.Not, x);
		}

		public static ClauseItem operator ~(ClauseItem x)
		{
			return new UnaryOperation(SimpleOperator.Complement, x);
		}

		public static ClauseItem operator ++(ClauseItem x)
		{
			throw new NotSupportedException();
		}

		public static ClauseItem operator --(ClauseItem x)
		{
			throw new NotSupportedException();
		}

		public static bool operator true(ClauseItem x)
		{
			return false;
		}

		public static bool operator false(ClauseItem x)
		{
			return false;
		}

		#endregion

		#region binary operations

		//+, -, *, /, %, &, |, ^, <<, >>
		public static ClauseItem operator +(ClauseItem x, ClauseItem y)
		{
			if (EvaluatesToString(x) || EvaluatesToString(y))
				return new ConcatenationOperation(x, y);
			else
				return new BinaryOperation(SimpleOperator.Add, x, y);
		}

		public static ClauseItem operator -(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(SimpleOperator.Subtract, x, y);
		}

		public static ClauseItem operator *(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(SimpleOperator.Multiply, x, y);
		}

		public static ClauseItem operator /(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(SimpleOperator.Divide, x, y);
		}

		public static ClauseItem operator %(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(SimpleOperator.Mod, x, y);
		}

		public static ClauseItem operator &(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(SimpleOperator.And, x, y);
		}

		public static ClauseItem operator |(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(SimpleOperator.Or, x, y);
		}

		public static ClauseItem operator ^(ClauseItem x, ClauseItem y)
		{
			throw new NotSupportedException();
		}

		public static ClauseItem operator <<(ClauseItem x, int y)
		{
			throw new NotSupportedException();
		}

		public static ClauseItem operator >>(ClauseItem x, int y)
		{
			throw new NotSupportedException();
		}

		//==, !=, <, >, <=, >=
		public static ClauseItem operator ==(ClauseItem x, ClauseItem y)
		{
			if (ReferenceEquals(x, null))
				return new NullComparisonOperation(true, y);
			else if (ReferenceEquals(y, null))
				return new NullComparisonOperation(true, x);
			else
				return new BinaryOperation(SimpleOperator.Equal, x, y);
		}

		public static ClauseItem operator !=(ClauseItem x, ClauseItem y)
		{
			if (ReferenceEquals(x, null))
				return new NullComparisonOperation(false, y);
			else if (ReferenceEquals(y, null))
				return new NullComparisonOperation(false, x);
			else
				return new BinaryOperation(SimpleOperator.NotEqual, x, y);
		}

		public static ClauseItem operator <(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(SimpleOperator.LessThan, x, y);
		}

		public static ClauseItem operator >(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(SimpleOperator.GreaterThan, x, y);
		}

		public static ClauseItem operator <=(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(SimpleOperator.LessThanOrEqual, x, y);
		}

		public static ClauseItem operator >=(ClauseItem x, ClauseItem y)
		{
			return new BinaryOperation(SimpleOperator.GreaterThanOrEqual, x, y);
		}

		public ClauseItem Like(ClauseItem clauseItem)
		{
			return new BinaryOperation(SimpleOperator.Like, this, clauseItem);
		}

		#endregion

		public ClauseItem In(params ClauseItem[] list)
		{
			return new InOperation(this, list);
		}

		public ClauseItem To<T>()
		{
			return new ConvertOperation(this, typeof (T));
		}

		public new ClauseItem Equals(object obj)
		{
			if (obj is ClauseGetter)
				throw new ArgumentException("Cant evaluate clause");

			ClauseItem clauseItem = obj as ClauseItem ?? new Constant(obj);

			return this == clauseItem;
		}

		public override int GetHashCode()
		{
			throw new NotSupportedException();
		}

		private static bool EvaluatesToString(ClauseItem clauseItem)
		{
			return (clauseItem is Constant && ((Constant) clauseItem).Object is string) || clauseItem is ConcatenationOperation;
		}
	}
}
