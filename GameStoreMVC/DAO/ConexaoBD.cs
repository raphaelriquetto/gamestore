using System.Data.Common;
using Npgsql;
//using System.Data.SqlClient;

namespace GameStoreMVC.DAO
{
    public static class ConexaoBD
    {
        // POSTGRES (testes em casa)
        private const string StrCon = "Server=localhost;Port=5432;Database=gamestore;User Id=postgres;Password=123456;Enlist=true;";
        public static readonly DbProviderFactory Fabrica = NpgsqlFactory.Instance;

        // SQL SERVER (apresentação) - comente as 2 linhas do Postgres acima e descomente as 2 abaixo
        //private const string StrCon = "Data Source=LOCALHOST;Initial Catalog=GAMESTORE;user id=sa;password=123456";
        //public static readonly DbProviderFactory Fabrica = SqlClientFactory.Instance;

        // retorna uma conexão aberta com o BD
        public static DbConnection GetConexao()
        {
            DbConnection conexao = Fabrica.CreateConnection();
            conexao.ConnectionString = StrCon;
            conexao.Open();
            return conexao;
        }
    }
}
