using System;
using System.Data;
using System.Dynamic;

namespace Brawndo.DynamicLinq
{
	public class DB : DynamicObject
	{
		private readonly Func<IDbConnection> getConnection;

		internal Func<IDbConnection> GetConnection
		{
			get { return getConnection; }
		}

		public DB(Func<IDbConnection> getConnection)
		{
			this.getConnection = getConnection;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = new Query(this, binder.Name);

			return true;
		}
	}
}
