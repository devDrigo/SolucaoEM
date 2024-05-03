using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;//preciso mudar pra algo mais genrico depois


namespace EM.REPOSITORY
{
	public class BDConnect
	{
		private static string _caminho = @"Database=C:\COPE.FDB;User=SYSDBA;Password=masterkey;DataSource=localhost;Port=3054;";
		private static FbConnection? conn = null;


		public static FbConnection GetConexao()
		{
			if (conn == null || conn.State != System.Data.ConnectionState.Open)
			{
				FbConnection.ClearAllPools();
				conn = new FbConnection(_caminho);
				conn.Open();
			}

			return conn;
		}
	}
}
