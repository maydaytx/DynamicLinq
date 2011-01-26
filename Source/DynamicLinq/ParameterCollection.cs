using System.Collections;
using System.Collections.Generic;

namespace DynamicLinq
{
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
}