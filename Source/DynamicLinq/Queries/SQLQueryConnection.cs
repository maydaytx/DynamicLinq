using System;
using System.Data;
using DynamicLinq.Collections;

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

		protected override bool Read(out object obj)
		{
			if (reader.Read())
			{
				if (QueryInfo.IsSingleColumnSelect)
				{
					obj = GetColumn(reader.GetName(0), reader.GetValue(0)).Item2;
				}
				else
				{
					DynamicBag dynamicBag = new DynamicBag();

					for (int i = 0; i < reader.FieldCount; ++i)
					{
						Tuple<string, object> value = GetColumn(reader.GetName(i), reader.GetValue(i));

						dynamicBag.SetValue(value.Item1, value.Item2);
					}

					obj = dynamicBag;
				}

				return true;
			}
			else
			{
				obj = null;
				return false;
			}
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