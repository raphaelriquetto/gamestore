using System;
using System.Collections.Generic;
using System.Linq;
using GameStoreMVC.DAO;
using GameStoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStoreMVC.Controllers
{
    // pedidos na área administrativa (listagem, detalhe e consultas avançadas)
    public class PedidoController : PadraoController<PedidoViewModel>
    {
        public PedidoController()
        {
            DAO = new PedidoDAO();
        }

        // pedidos são criados pelo carrinho, aqui o admin só consulta e altera o status
        public override IActionResult Create() => RedirectToAction("Index");
        public override IActionResult Save(PedidoViewModel model, string Operacao) => RedirectToAction("Index");
        public override IActionResult Edit(int id) => RedirectToAction("Detalhes", new { id });
        public override IActionResult Delete(int id) => RedirectToAction("Index");

        // mestre-detalhe: dados do pedido + itens
        public IActionResult Detalhes(int id)
        {
            try
            {
                var pedido = DAO.Consulta(id);
                if (pedido == null)
                    return RedirectToAction("Index");

                pedido.Itens = new PedidoItemDAO().ListagemPorPedido(id);
                ViewBag.Cliente = new ClienteDAO().Consulta(pedido.ClienteId);
                ViewBag.ListaStatus = new SelectList(PedidoViewModel.ListaStatus, pedido.Status);
                return View(pedido);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        // AJAX: altera o status do pedido
        [HttpPost]
        public IActionResult AlterarStatus(int id, string status)
        {
            try
            {
                if (!PedidoViewModel.ListaStatus.Contains(status))
                    return Json(new { erro = true, msg = "Status inválido." });

                ((PedidoDAO)DAO).AlteraStatus(id, status);
                return Json(new { erro = false, msg = "Status alterado para " + status + "." });
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        // tela de consulta 1: pedidos
        public IActionResult ConsultaPedidos()
        {
            var lista = new List<SelectListItem> { new SelectListItem("TODOS", "") };
            foreach (string status in PedidoViewModel.ListaStatus)
                lista.Add(new SelectListItem(status, status));
            ViewBag.ListaStatus = lista;
            return View();
        }

        // AJAX: devolve a partial view com o resultado da consulta 1
        public IActionResult ObtemDadosConsultaPedidos(string nomeCliente, string status, DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                AjustaPeriodo(ref dataInicial, ref dataFinal);
                var lista = ((PedidoDAO)DAO).ConsultaAvancada(nomeCliente ?? "", status ?? "", dataInicial, dataFinal);
                return PartialView("pvGridPedidos", lista);
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        // tela de consulta 2: jogos vendidos
        public IActionResult ConsultaVendas()
        {
            var categorias = new List<SelectListItem> { new SelectListItem("TODAS", "0") };
            foreach (var c in new CategoriaDAO().Listagem())
                categorias.Add(new SelectListItem(c.Nome, c.Id.ToString()));

            var plataformas = new List<SelectListItem> { new SelectListItem("TODAS", "0") };
            foreach (var p in new PlataformaDAO().Listagem())
                plataformas.Add(new SelectListItem(p.Nome, p.Id.ToString()));

            ViewBag.Categorias = categorias;
            ViewBag.Plataformas = plataformas;
            return View();
        }

        // AJAX: devolve a partial view com o resultado da consulta 2
        public IActionResult ObtemDadosConsultaVendas(string nomeJogo, int categoriaId, int plataformaId, DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                AjustaPeriodo(ref dataInicial, ref dataFinal);
                var lista = new PedidoItemDAO().ConsultaVendas(nomeJogo ?? "", categoriaId, plataformaId, dataInicial, dataFinal);
                return PartialView("pvGridVendas", lista);
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        // datas não informadas viram um período bem amplo; a data final vai até 23:59:59
        private void AjustaPeriodo(ref DateTime dataInicial, ref DateTime dataFinal)
        {
            if (dataInicial == DateTime.MinValue)
                dataInicial = new DateTime(1900, 1, 1);
            if (dataFinal == DateTime.MinValue)
                dataFinal = new DateTime(9999, 12, 31);
            else
                dataFinal = dataFinal.Date.AddDays(1).AddSeconds(-1);
        }
    }
}
