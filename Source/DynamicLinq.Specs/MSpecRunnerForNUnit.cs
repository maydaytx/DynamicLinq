using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications;
using NUnit.Framework;

namespace DynamicLinq
{
	[TestFixture]
	public class MSpecRunnerForNUnit
	{
		private IEnumerable<object[]> TestFixtures
		{
			get
			{
				return typeof (MSpecRunnerForNUnit).Assembly.GetTypes()
					.Select(x => new {Type = x, Fields = x.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)})
					.Where(x => x.Fields.Any(y => y.FieldType == typeof (It)))
					.Select(x => new object[] {x.Type, x.Fields});
			}
		}
		
		[Test]
		[TestCaseSource("TestFixtures")]
		public void Run_Machine_Specification(Type type, FieldInfo[] fields)
		{
			var contextField = fields.Single(x => x.FieldType == typeof (Establish));
			var ofField = fields.Single(x => x.FieldType == typeof (Because));
			var testFields = fields.Where(x => x.FieldType == typeof (It));

			var testFixtureInstance = Activator.CreateInstance(type);

			foreach (var testField in testFields)
			{
				if (contextField != null)
				{
					var context = (Establish) contextField.GetValue(testFixtureInstance);

					context();
				}

				if (ofField != null)
				{
					var of = (Because) ofField.GetValue(testFixtureInstance);

					of();
				}

				var test = (It) testField.GetValue(testFixtureInstance);

				test();
			}
		}
	}
}
