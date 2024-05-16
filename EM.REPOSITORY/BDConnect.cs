using FirebirdSql.Data.FirebirdClient;


namespace EM.REPOSITORY
{
    public class BDConnect
    {
        //pego o caminho relativo para nao precisar mudar quando abro em computador diferente.
        private static string _caminhoRelativo = @"\COPE.FDB";
        private static string _caminhoAbsoluto = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _caminhoRelativo);

        private static string _caminho = $@"Database={_caminhoAbsoluto};User=SYSDBA;Password=masterkey;DataSource=localhost;Port=3054;";

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