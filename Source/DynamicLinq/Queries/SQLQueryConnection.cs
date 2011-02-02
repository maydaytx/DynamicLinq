using System;
using System.Data;

namespace DynamicLinq.Queries
{
	internal class SQLQueryConnection : QueryConnection
	{
		private IDbConnection connection;
		private IDbCommand command;
		private IDataReader reader;

		internal SQLQueryConnection(Func<IDbConnection> getConnection, QueryInfo queryInfo) : base(queryInfo)
		{
			connection = getConnection();
			connection.Open();

			command = connection.CreateCommand();

			command.CommandText = queryInfo.Query;

			foreach (Parameter parameter in queryInfo.Parameters)
			{
				IDbDataParameter dataParameter = command.CreateParameter();

				dataParameter.ParameterName = parameter.Name;
				dataParameter.Value = parameter.Value;

				command.Parameters.Add(dataParameter);
			}

			reader = command.ExecuteReader();
		}

		protected override IQueryReader Reader
		{
			get { return new SQLQueryReader(reader); }
		}

		public override void Dispose()
		{
			if (reader != null)
			{
				reader.Dispose();
				reader = null;
			}

			if (command != null)
			{
				command.Dispose();
				command = null;
			}

			if (connection != null)
			{
				connection.Dispose();
				connection = null;
			}

			IsDisposed = true;
		}

		private class SQLQueryReader : IQueryReader
		{
			private readonly IDataReader reader;

			public SQLQueryReader(IDataReader reader)
			{
				this.reader = reader;
			}

			public bool Read()
			{
				return reader.Read();
			}

			public int FieldCount
			{
				get { return reader.FieldCount; }
			}

			public string GetName(int index)
			{
				return reader.GetName(index);
			}

			public object GetValue(int index)
			{
				return reader.GetValue(index);
			}
		}
	}
}