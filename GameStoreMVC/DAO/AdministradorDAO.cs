using System.Data.Common;

namespace GameStoreMVC.DAO
{
    public class AdministradorDAO
    {
        // devolve o nome do administrador quando usuário e senha conferem
        public string Login(string usuario, string senhaHash)
        {
            var p = new DbParameter[]
            {
                HelperDAO.CriaParametro("usuario", usuario),
                HelperDAO.CriaParametro("senha", senhaHash)
            };
            var tabela = HelperDAO.ExecutaProcSelect("spLoginAdministrador", p);
            if (tabela.Rows.Count == 0)
                return null;
            else
                return tabela.Rows[0]["nome"].ToString();
        }
    }
}
