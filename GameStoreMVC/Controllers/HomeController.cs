using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameStoreMVC.DAO;
using GameStoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStoreMVC.Controllers
{
    // home da área do cliente
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var categorias = new List<SelectListItem> { new SelectListItem("Todas as categorias", "0") };
                foreach (var c in new CategoriaDAO().Listagem())
                    categorias.Add(new SelectListItem(c.Nome, c.Id.ToString()));

                var plataformas = new List<SelectListItem> { new SelectListItem("Todas as plataformas", "0") };
                foreach (var p in new PlataformaDAO().Listagem())
                    plataformas.Add(new SelectListItem(p.Nome, p.Id.ToString()));

                ViewBag.Categorias = categorias;
                ViewBag.Plataformas = plataformas;

                var jogos = new JogoDAO().ListagemVitrine("", 0, 0);
                return View(jogos);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        // AJAX: filtra os jogos da vitrine sem recarregar a página
        public IActionResult FiltraVitrine(string nome, int categoriaId, int plataformaId)
        {
            try
            {
                var jogos = new JogoDAO().ListagemVitrine(nome, categoriaId, plataformaId);
                return PartialView("pvVitrine", jogos);
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        public IActionResult Detalhes(int id)
        {
            try
            {
                var jogo = new JogoDAO().Consulta(id);
                if (jogo == null)
                    return RedirectToAction("Index");

                jogo.NomeCategoria = new CategoriaDAO().Consulta(jogo.CategoriaId)?.Nome;
                jogo.NomePlataforma = new PlataformaDAO().Consulta(jogo.PlataformaId)?.Nome;
                return View(jogo);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult Sobre()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
