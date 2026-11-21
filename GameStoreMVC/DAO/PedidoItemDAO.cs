using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using GameStoreMVC.Models;

namespace GameStoreMVC.DAO
{
    public class PedidoItemDAO : PadraoDAO<PedidoItemViewModel>
    {
        protected override DbParameter[] CriaParametros(PedidoItemViewModel model)
        {
            DbParameter[] parametros =
            {
                HelperDAO.CriaParametro("id", model.Id),
                HelperDAO.CriaParametro("pedidoId", model.PedidoId),
                HelperDAO.CriaParametro("jogoId", model.JogoId),
                HelperDAO.CriaParametro("qtde", model.Qtde),
                HelperDAO.CriaParametro("valorUnitario", model.ValorUnitario)
            };
            return parametros;
        }

        protected override PedidoItemViewModel MontaModel(DataRow registro)
        {
            PedidoItemViewModel i = new PedidoItemViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                PedidoId = Convert.ToInt32(registro["pedidoId"]),
                JogoId = Convert.ToInt32(registro["jogoId"]),
                Qtde = Convert.ToInt32(registro["qtde"]),
                ValorUnitario = Convert.ToDecimal(registro["valorUnitario"])
            };

            if (registro.Table.Columns.Contains("NomeJogo"))
                i.NomeJogo = registro["NomeJogo"].ToString();
            if (registro.Table.Columns.Contains("NomeCategoria"))
                i.NomeCategoria = registro["NomeCategoria"].ToString();
            if (registro.Table.Columns.Contains("NomePlataforma"))
                i.NomePlataforma = registro["NomePlataforma"].ToString();
            if (registro.Table.Columns.Contains("NomeCliente"))
                i.NomeCliente = registro["NomeCliente"].ToString();
            if (registro.Table.Columns.Contains("DataPedido"))
                i.DataPedido = Convert.ToDateTime(registro["DataPedido"]);

            return i;
        }

        protected override void SetTabela()
        {
            Tabela = "PedidoItens";
            ChaveIdentity = true;
        }

        public List<PedidoItemViewModel> ListagemPorPedido(int pedidoId)
        {
            var p = new DbParameter[] { HelperDAO.CriaParametro("pedidoId", pedidoId) };
            var tabela = HelperDAO.ExecutaProcSelect("spListagemItensPedido", p);
            return MontaLista(tabela);
        }

        // consulta avançada de jogos vendidos (tela de consulta 2)
        public List<PedidoItemViewModel> ConsultaVendas(string nomeJogo, int categoriaId, int plataformaId, DateTime dataInicial, DateTime dataFinal)
        {
            DbParameter[] p =
            {
                HelperDAO.CriaParametro("nomeJogo", nomeJogo),
                HelperDAO.CriaParametro("categoriaId", categoriaId),
                HelperDAO.CriaParametro("plataformaId", plataformaId),
                HelperDAO.CriaParametro("dataInicial", dataInicial),
                HelperDAO.CriaParametro("dataFinal", dataFinal)
            };
            var tabela = HelperDAO.ExecutaProcSelect("spConsultaAvancadaVendas", p);
            return MontaLista(tabela);
        }
    }
}
