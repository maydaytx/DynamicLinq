using System;
using System.Data;
using System.Dynamic;
using Brawndo.DynamicLinq.Dialect;

namespace Brawndo.DynamicLinq
{
	public class DB : DynamicObject
	{
		private readonly Func<IDbConnection> getConnection;
		private readonly SQLDialect dialect;

		internal Func<IDbConnection> GetConnection
		{
			get { return getConnection; }
		}

		internal SQLDialect Dialect
		{
			get { return dialect; }
		}

		public DB(Func<IDbConnection> getConnection, SQLDialect dialect)
		{
			this.getConnection = getConnection;
			this.dialect = dialect;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = new DatabaseOperation(this, binder.Name);

			return true;
		}
	}
}
