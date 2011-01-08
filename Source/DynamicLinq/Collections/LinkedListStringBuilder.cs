using System.Text;

namespace DynamicLinq.Collections
{
	public class LinkedListStringBuilder
	{
		private Node first;
		private Node last;
		private int count;

		public LinkedListStringBuilder(string value)
		{
			count = 1;
			first = last = value;
		}

		public LinkedListStringBuilder() { }

		public void Append(LinkedListStringBuilder list)
		{
			if (count > 0)
				last.Next = list.first;
			else
				first = list.first;

			count += list.count;
			last = list.last;
		}

		public void Append(string value)
		{
			++count;
			Node node = value;

			if (last == null)
			{
				first = last = node;
			}
			else
			{
				last.Next = node;
				last = node;
			}
		}

		public void Prepend(string value)
		{
			++count;
			Node node = value;

			if (first == null)
			{
				first = last = node;
			}
			else
			{
				node.Next = first;
				first = node;
			}
		}

		public static implicit operator LinkedListStringBuilder(string str)
		{
			return new LinkedListStringBuilder(str);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(count);

			Node node = first;

			while (node != null)
			{
				sb.Append(node.Value);

				node = node.Next;
			}

			return sb.ToString();
		}

		private class Node
		{
			public Node Next;
			public readonly string Value;

			private Node(string value)
			{
				Value = value;
			}

			public static implicit operator Node(string value)
			{
				return new Node(value);
			}
		}
	}
}
