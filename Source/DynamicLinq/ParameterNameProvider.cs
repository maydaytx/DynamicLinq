using System.Collections;
using System.Collections.Generic;

namespace DynamicLinq
{
	internal class ParameterNameProvider
	{
		private readonly Dialect dialect;
		private int count;

		internal ParameterNameProvider(Dialect dialect)
		{
			this.dialect = dialect;
		}

		internal string GetParameterName()
		{
			return dialect.ParameterPrefix + "p" + count++;
		}
	}

	public class ParameterCollection : IEnumerable<Parameter>
	{
		private readonly ParameterNameProvider nameProvider;
		private readonly IList<Parameter> parameters = new List<Parameter>();

		internal ParameterCollection(ParameterNameProvider nameProvider)
		{
			this.nameProvider = nameProvider;
		}

		public string Add(object value)
		{
			string parameterName = nameProvider.GetParameterName();

			parameters.Add(new Parameter(parameterName, value));

			return parameterName;
		}

		IEnumerator<Parameter> IEnumerable<Parameter>.GetEnumerator()
		{
			return parameters.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Parameter>) this).GetEnumerator();
		}
	}

	public class Parameter
	{
		public string Name { get; private set; }
		public object Value { get; private set; }

		public Parameter(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}
}