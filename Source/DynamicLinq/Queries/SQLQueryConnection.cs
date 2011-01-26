using System.Data;
using DynamicLinq.Dialects;

namespace DynamicLinq.Queries
{
	internal class SQLQueryConnection : QueryConnection
	{
		private IDbConnection connection;
		private IDbCommand command;
		private IDataReader reader;

		internal SQLQueryConnection(SQLDialect dialect, QueryInfo queryInfo) : base(queryInfo)
		{
			connection = dialect.GetConnection();
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

		protected override IDataReader Reader
		{
			get { return reader; }
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
	}
}