using System;
using System.Data;
using System.Data.Common;
using GameStoreMVC.Models;

namespace GameStoreMVC.DAO
{
    public class ClienteDAO : PadraoDAO<ClienteViewModel>
    {
        protected override DbParameter[] CriaParametros(ClienteViewModel model)
        {
            DbParameter[] parametros =
            {
                HelperDAO.CriaParametro("id", model.Id),
                HelperDAO.CriaParametro("nome", model.Nome),
                HelperDAO.CriaParametro("email", model.Email),
                HelperDAO.CriaParametro("senha", model.Senha),
                HelperDAO.CriaParametro("cpf", model.Cpf),
                HelperDAO.CriaParametro("telefone", model.Telefone),
                HelperDAO.CriaParametro("cep", model.Cep),
                HelperDAO.CriaParametro("logradouro", model.Logradouro),
                HelperDAO.CriaParametro("numero", model.Numero),
                HelperDAO.CriaParametro("bairro", model.Bairro),
                HelperDAO.CriaParametro("cidade", model.Cidade),
                HelperDAO.CriaParametro("uf", model.Uf)
            };
            return parametros;
        }

        protected override ClienteViewModel MontaModel(DataRow registro)
        {
            ClienteViewModel c = new ClienteViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                Nome = registro["nome"].ToString(),
                Email = registro["email"].ToString(),
                Senha = registro["senha"].ToString(),
                Cpf = registro["cpf"].ToString(),
                Telefone = registro["telefone"].ToString(),
                Cep = registro["cep"].ToString(),
                Logradouro = registro["logradouro"].ToString(),
                Numero = registro["numero"].ToString(),
                Bairro = registro["bairro"].ToString(),
                Cidade = registro["cidade"].ToString(),
                Uf = registro["uf"].ToString()
            };
            return c;
        }

        protected override void SetTabela()
        {
            Tabela = "Clientes";
        }

        public ClienteViewModel ConsultaPorEmail(string email)
        {
            var p = new DbParameter[] { HelperDAO.CriaParametro("email", email) };
            var tabela = HelperDAO.ExecutaProcSelect("spConsultaClientePorEmail", p);
            if (tabela.Rows.Count == 0)
                return null;
            else
                return MontaModel(tabela.Rows[0]);
        }

        // devolve o cliente quando email e senha (hash) conferem
        public ClienteViewModel Login(string email, string senhaHash)
        {
            ClienteViewModel cliente = ConsultaPorEmail(email);
            if (cliente != null && cliente.Senha == senhaHash)
                return cliente;
            return null;
        }
    }
}
