using System;
using System.Data;
using System.Data.Common;
using GameStoreMVC.Models;

namespace GameStoreMVC.DAO
{
    public class CategoriaDAO : PadraoDAO<CategoriaViewModel>
    {
        protected override DbParameter[] CriaParametros(CategoriaViewModel model)
        {
            DbParameter[] parametros =
            {
                HelperDAO.CriaParametro("id", model.Id),
                HelperDAO.CriaParametro("nome", model.Nome)
            };
            return parametros;
        }

        protected override CategoriaViewModel MontaModel(DataRow registro)
        {
            CategoriaViewModel c = new CategoriaViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                Nome = registro["nome"].ToString()
            };
            return c;
        }

        protected override void SetTabela()
        {
            Tabela = "Categorias";
        }
    }
}
