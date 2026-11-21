using System;
using System.Data;
using System.Data.Common;
using GameStoreMVC.Models;

namespace GameStoreMVC.DAO
{
    public class PlataformaDAO : PadraoDAO<PlataformaViewModel>
    {
        protected override DbParameter[] CriaParametros(PlataformaViewModel model)
        {
            DbParameter[] parametros =
            {
                HelperDAO.CriaParametro("id", model.Id),
                HelperDAO.CriaParametro("nome", model.Nome),
                HelperDAO.CriaParametro("fabricante", model.Fabricante)
            };
            return parametros;
        }

        protected override PlataformaViewModel MontaModel(DataRow registro)
        {
            PlataformaViewModel p = new PlataformaViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                Nome = registro["nome"].ToString(),
                Fabricante = registro["fabricante"].ToString()
            };
            return p;
        }

        protected override void SetTabela()
        {
            Tabela = "Plataformas";
        }
    }
}
