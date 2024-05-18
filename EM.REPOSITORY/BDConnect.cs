using FirebirdSql.Data.FirebirdClient;


namespace EM.REPOSITORY
{
    public class BDConnect
    {
        private static string _caminho = $@"Server=52.67.33.12;Database=cope;User=SYSDBA;Password=masterkey;Port=3050;";

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