using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Machine.Specifications;

namespace DynamicLinq.Specs
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

			result1 = results[0].Serialize().Deserialize<object>();
			result2 = results[1].Serialize().Deserialize<object>();

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
}
