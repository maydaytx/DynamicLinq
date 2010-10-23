﻿using System.Text;

namespace DynamicLinq
{
	internal class LinkedListStringBuilder
	{
		private int count;
		private Node<string> first;
		private Node<string> last;

		internal LinkedListStringBuilder(string str)
		{
			count = 1;
			first = last = new Node<string>(str);
		}

		internal LinkedListStringBuilder() { }

		public static LinkedListStringBuilder operator +(LinkedListStringBuilder x, LinkedListStringBuilder y)
		{
			if (x.last == null)
			{
				return y;
			}
			else if (y.first == null)
			{
				return x;
			}
			else
			{
				x.last.next = y.first;
				x.count += y.count;
				x.last = y.last;
				y.count = x.count;
				y.first = x.first;

				return x;
			}
		}

		public static LinkedListStringBuilder operator +(string x, LinkedListStringBuilder y)
		{
			++y.count;
			Node<string> node = new Node<string>(x);

			if (y.first == null)
			{
				y.first = y.last = node;
			}
			else
			{
				node.next = y.first;
				y.first = node;
			}

			return y;
		}

		public static LinkedListStringBuilder operator +(LinkedListStringBuilder x, string y)
		{
			++x.count;
			Node<string> node = new Node<string>(y);

			if (x.last == null)
			{
				x.first = x.last = node;
			}
			else
			{
				x.last.next = node;
				x.last = node;
			}

			return x;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(count);

			Node<string> node = first;

			while (node != null)
			{
				sb.Append(node.value);

				node = node.next;
			}

			return sb.ToString();
		}

		private class Node<T>
		{
			public Node<T> next;
			public readonly string value;

			public Node(string str)
			{
				value = str;
			}
		}
	}
}
