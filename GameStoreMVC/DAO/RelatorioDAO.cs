using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using GameStoreMVC.Models;

namespace GameStoreMVC.DAO
{
    // consultas consolidadas usadas pelas APIs
    public class RelatorioDAO
    {
        public List<VendaCategoriaViewModel> VendasPorCategoria(DateTime dataInicial, DateTime dataFinal)
        {
            DbParameter[] p =
            {
                HelperDAO.CriaParametro("dataInicial", dataInicial),
                HelperDAO.CriaParametro("dataFinal", dataFinal)
            };
            var tabela = HelperDAO.ExecutaProcSelect("spVendasPorCategoria", p);

            var lista = new List<VendaCategoriaViewModel>();
            foreach (DataRow r in tabela.Rows)
            {
                lista.Add(new VendaCategoriaViewModel
                {
                    Categoria = r["categoria"].ToString(),
                    QtdeVendida = Convert.ToInt32(r["qtdeVendida"]),
                    ValorTotal = Convert.ToDecimal(r["valorTotal"])
                });
            }
            return lista;
        }

        public List<JogoMaisVendidoViewModel> JogosMaisVendidos(int top)
        {
            var p = new DbParameter[] { HelperDAO.CriaParametro("top", top) };
            var tabela = HelperDAO.ExecutaProcSelect("spJogosMaisVendidos", p);

            var lista = new List<JogoMaisVendidoViewModel>();
            foreach (DataRow r in tabela.Rows)
            {
                lista.Add(new JogoMaisVendidoViewModel
                {
                    JogoId = Convert.ToInt32(r["jogoId"]),
                    Jogo = r["jogo"].ToString(),
                    Plataforma = r["plataforma"].ToString(),
                    QtdeVendida = Convert.ToInt32(r["qtdeVendida"]),
                    ValorTotal = Convert.ToDecimal(r["valorTotal"])
                });
            }
            return lista;
        }

        public ResumoVendasViewModel Resumo()
        {
            var tabela = HelperDAO.ExecutaProcSelect("spResumoVendas", null);
            DataRow r = tabela.Rows[0];
            return new ResumoVendasViewModel
            {
                TotalPedidos = Convert.ToInt32(r["totalPedidos"]),
                Faturamento = Convert.ToDecimal(r["faturamento"]),
                TicketMedio = Convert.ToDecimal(r["ticketMedio"]),
                TotalClientes = Convert.ToInt32(r["totalClientes"]),
                TotalJogos = Convert.ToInt32(r["totalJogos"])
            };
        }
    }
}
