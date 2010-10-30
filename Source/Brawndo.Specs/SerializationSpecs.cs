using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Brawndo.DynamicLinq;
using Brawndo.Emit;
using Machine.Specifications;

namespace Brawndo.Specs
{
	internal static class SerializationExtensions
	{
		public static byte[] Serialize(this object obj)
		{
			byte[] bytes;

			using (MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();

				formatter.Serialize(stream, obj);

				stream.Flush();

				bytes = stream.ToArray();
			}

			return bytes;
		}

		public static T Deserialize<T>(this byte[] bytes)
		{
			T obj;

			using (MemoryStream stream = new MemoryStream())
			{
				stream.Write(bytes, 0, bytes.Length);
				stream.Flush();

				BinaryFormatter formatter = new BinaryFormatter();

				Duck.Configure(formatter);

				stream.Position = 0;

				obj = (T) formatter.Deserialize(stream);
			}

			return obj;
		}
	}

	public class When_deserializing_results
	{
		private static dynamic db;
		private static object result1;
		private static object result2;
		private static dynamic deserializedResult1;
		private static dynamic deserializedResult2;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name], [Time] DATETIME);

INSERT INTO [Table] ([Id], [Name], [Time]) VALUES (1, 'Name1', '2012-12-21');
INSERT INTO [Table] ([Id], [Name], [Time]) VALUES (2, 'Name2', '2001-09-11');"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			IList<object> results = (from record in (object) db.Table
			                         select new {record.Name, record.Time}).ToList();

			result1 = results[0];
			result2 = results[1];

			deserializedResult1 = result1.Serialize().Deserialize<object>();
			deserializedResult2 = result2.Serialize().Deserialize<object>();
		};

		It should_create_a_duck_type_with_the_same_properties = () =>
		{
			((object)deserializedResult1.Name).ShouldEqual("Name1");
			((object)deserializedResult1.Time).ShouldEqual(new DateTime(2012, 12, 21));
			((object)deserializedResult2.Name).ShouldEqual("Name2");
			((object)deserializedResult2.Time).ShouldEqual(new DateTime(2001, 09, 11));
		};

		It should_be_the_same_type = () =>
		{
			((object)deserializedResult1).ShouldBeOfType(result1.GetType());
			((object)deserializedResult2).ShouldBeOfType(result2.GetType());
			result1.ShouldBeOfType(result2.GetType());
		};
	}

	public class When_deserializing_unenumerated_results
	{
		private static dynamic db;
		private static dynamic deserializedResult1;
		private static dynamic deserializedResult2;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name], [Time] DATETIME);

INSERT INTO [Table] ([Id], [Name], [Time]) VALUES (1, 'Name1', '2012-12-21');
INSERT INTO [Table] ([Id], [Name], [Time]) VALUES (2, 'Name2', '2001-09-11');"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			IEnumerable<object> results = (from record in (object) db.Table
			                               select new {record.Name, record.Time});

			IList<object> deserializedResults = results.Serialize().Deserialize<IEnumerable<object>>().ToList();

			deserializedResult1 = deserializedResults[0].Serialize().Deserialize<object>();
			deserializedResult2 = deserializedResults[1].Serialize().Deserialize<object>();
		};

		It should_create_a_duck_type_with_the_same_properties = () =>
		{
			((object)deserializedResult1.Name).ShouldEqual("Name1");
			((object)deserializedResult1.Time).ShouldEqual(new DateTime(2012, 12, 21));
			((object)deserializedResult2.Name).ShouldEqual("Name2");
			((object)deserializedResult2.Time).ShouldEqual(new DateTime(2001, 09, 11));
		};
	}

	public class When_deserializing_results_with_nullable_columns_in_different_app_domains
	{
		private static dynamic db;
		private static dynamic deserializedResult1;
		private static dynamic deserializedResult2;
		private static dynamic deserializedResult3;

		Establish context = () =>
		{
			var getConnection = SQLite.CreateInMemoryDatabase
			(
@"CREATE TABLE [Table] ([Id] PRIMARY KEY, [Name], [Value]);

INSERT INTO [Table] ([Id], [Name]) VALUES (1, 'Name1');
INSERT INTO [Table] ([Id], [Name], [Value]) VALUES (2, 'Name2', 1);
INSERT INTO [Table] ([Id], [Name]) VALUES (3, 'Name3');"
			);

			db = new DB(getConnection);
		};

		Because of = () =>
		{
			byte[] serializedResults = (from record in (object) db.Table
			                            select new {record.Name, record.Value}).ToList().Serialize();

			DuckRepository.ResetCache();

			IList<object> deserializedResults = serializedResults.Deserialize<IList<object>>();

			deserializedResult1 = deserializedResults[0];
			deserializedResult2 = deserializedResults[1];
			deserializedResult3 = deserializedResults[2];
		};

		It should_create_a_duck_type_with_the_same_properties = () =>
		{
			((object)deserializedResult1.Name).ShouldEqual("Name1");
			((object)deserializedResult1.Value).ShouldEqual(null);
			((object)deserializedResult2.Name).ShouldEqual("Name2");
			((object)deserializedResult2.Value).ShouldEqual(1L);
			((object)deserializedResult3.Name).ShouldEqual("Name3");
			((object)deserializedResult3.Value).ShouldEqual(null);
		};

		It should_be_the_same_type = () =>
		{
			((object)deserializedResult1).ShouldBeOfType(((object)deserializedResult2).GetType());
			((object)deserializedResult1).ShouldBeOfType(((object)deserializedResult3).GetType());
		};
	}
}
