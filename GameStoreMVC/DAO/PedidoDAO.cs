using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using GameStoreMVC.Models;

namespace GameStoreMVC.DAO
{
    public class PedidoDAO : PadraoDAO<PedidoViewModel>
    {
        protected override DbParameter[] CriaParametros(PedidoViewModel model)
        {
            DbParameter[] parametros =
            {
                HelperDAO.CriaParametro("id", model.Id),
                HelperDAO.CriaParametro("data", model.Data),
                HelperDAO.CriaParametro("clienteId", model.ClienteId),
                HelperDAO.CriaParametro("valorTotal", model.ValorTotal),
                HelperDAO.CriaParametro("status", model.Status)
            };
            return parametros;
        }

        protected override PedidoViewModel MontaModel(DataRow registro)
        {
            PedidoViewModel p = new PedidoViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                Data = Convert.ToDateTime(registro["data"]),
                ClienteId = Convert.ToInt32(registro["clienteId"]),
                ValorTotal = Convert.ToDecimal(registro["valorTotal"]),
                Status = registro["status"].ToString()
            };

            if (registro.Table.Columns.Contains("NomeCliente"))
                p.NomeCliente = registro["NomeCliente"].ToString();

            if (registro.Table.Columns.Contains("QtdeItens"))
                p.QtdeItens = Convert.ToInt32(registro["QtdeItens"]);

            return p;
        }

        protected override void SetTabela()
        {
            Tabela = "Pedidos";
            ChaveIdentity = true;
        }

        public void AlteraStatus(int id, string status)
        {
            DbParameter[] p =
            {
                HelperDAO.CriaParametro("id", id),
                HelperDAO.CriaParametro("status", status)
            };
            HelperDAO.ExecutaProc("spAlteraStatusPedido", p);
        }

        public List<PedidoViewModel> ListagemPorCliente(int clienteId)
        {
            var p = new DbParameter[] { HelperDAO.CriaParametro("clienteId", clienteId) };
            var tabela = HelperDAO.ExecutaProcSelect("spListagemPedidosCliente", p);
            return MontaLista(tabela);
        }

        // consulta avançada de pedidos (tela de consulta 1)
        public List<PedidoViewModel> ConsultaAvancada(string nomeCliente, string status, DateTime dataInicial, DateTime dataFinal)
        {
            DbParameter[] p =
            {
                HelperDAO.CriaParametro("nomeCliente", nomeCliente),
                HelperDAO.CriaParametro("status", status),
                HelperDAO.CriaParametro("dataInicial", dataInicial),
                HelperDAO.CriaParametro("dataFinal", dataFinal)
            };
            var tabela = HelperDAO.ExecutaProcSelect("spConsultaAvancadaPedidos", p);
            return MontaLista(tabela);
        }
    }
}
