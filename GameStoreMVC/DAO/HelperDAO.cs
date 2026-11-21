using System;
using System.Data;
using System.Data.Common;

namespace GameStoreMVC.DAO
{
    public static class HelperDAO
    {
        // cria um parâmetro do banco configurado na ConexaoBD
        public static DbParameter CriaParametro(string nome, object valor, DbType tipoQuandoNulo = DbType.String)
        {
            DbParameter parametro = ConexaoBD.Fabrica.CreateParameter();
            // minúsculo porque o postgres diferencia maiúsculas no nome dos parâmetros
            parametro.ParameterName = nome.ToLower();
            if (valor == null)
            {
                // valores nulos precisam do tipo informado
                parametro.DbType = tipoQuandoNulo;
                parametro.Value = DBNull.Value;
            }
            else
                parametro.Value = valor;
            return parametro;
        }

        // executa uma stored procedure (insert, update, delete)
        public static int ExecutaProc(string nomeProc, DbParameter[] parametros, bool consultaUltimoIdentity = false)
        {
            using (DbConnection conexao = ConexaoBD.GetConexao())
            {
                using (DbCommand comando = conexao.CreateCommand())
                {
                    comando.CommandText = nomeProc;
                    comando.CommandType = CommandType.StoredProcedure;
                    if (parametros != null)
                        comando.Parameters.AddRange(parametros);

                    // procedures de tabelas com chave automática devolvem o id gerado
                    if (consultaUltimoIdentity)
                        return Convert.ToInt32(comando.ExecuteScalar());

                    comando.ExecuteNonQuery();
                    return 0;
                }
            }
        }

        // executa uma stored procedure que retorna registros
        public static DataTable ExecutaProcSelect(string nomeProc, DbParameter[] parametros)
        {
            using (DbConnection conexao = ConexaoBD.GetConexao())
            {
                using (DbDataAdapter adapter = ConexaoBD.Fabrica.CreateDataAdapter())
                {
                    adapter.SelectCommand = conexao.CreateCommand();
                    adapter.SelectCommand.CommandText = nomeProc;
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    if (parametros != null)
                        adapter.SelectCommand.Parameters.AddRange(parametros);

                    DataTable tabela = new DataTable();
                    adapter.Fill(tabela);
                    return tabela;
                }
            }
        }
    }
}
