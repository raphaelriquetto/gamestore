using System;
using GameStoreMVC.DAO;
using GameStoreMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreMVC.Controllers
{
    // login, cadastro e pedidos do cliente
    public class ContaController : Controller
    {
        public IActionResult Login(string retorno)
        {
            ViewBag.Retorno = retorno;
            return View();
        }

        [HttpPost]
        public IActionResult FazLogin(string email, string senha, string retorno)
        {
            try
            {
                ClienteViewModel cliente = null;
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(senha))
                    cliente = new ClienteDAO().Login(email, HelperControllers.HashSenha(senha));

                if (cliente == null)
                {
                    ViewBag.Erro = "E-mail ou senha inválidos!";
                    ViewBag.Retorno = retorno;
                    return View("Login");
                }

                HttpContext.Session.SetInt32("ClienteId", cliente.Id);
                HttpContext.Session.SetString("ClienteNome", cliente.Nome);

                // volta para o carrinho quando o login foi pedido no fechamento do pedido
                if (retorno == "carrinho")
                    return RedirectToAction("Index", "Carrinho");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult LogOff()
        {
            HttpContext.Session.Remove("ClienteId");
            HttpContext.Session.Remove("ClienteNome");
            HelperControllers.LimparCarrinho(HttpContext.Session);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Cadastro()
        {
            return View(new ClienteViewModel());
        }

        [HttpPost]
        public IActionResult SalvarCadastro(ClienteViewModel cliente)
        {
            try
            {
                ModelState.Clear();
                ClienteDAO dao = new ClienteDAO();
                cliente.Id = dao.ProximoId();
                ClienteController.ValidaCampos(cliente, "I", ModelState, dao);

                if (!ModelState.IsValid)
                {
                    cliente.Senha = null;
                    return View("Cadastro", cliente);
                }

                dao.Insert(cliente);
                HttpContext.Session.SetInt32("ClienteId", cliente.Id);
                HttpContext.Session.SetString("ClienteNome", cliente.Nome);
                TempData["Mensagem"] = "Cadastro realizado! Boas compras :)";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult MeusPedidos()
        {
            try
            {
                int? clienteId = HttpContext.Session.GetInt32("ClienteId");
                if (clienteId == null)
                    return RedirectToAction("Login");

                var pedidos = new PedidoDAO().ListagemPorCliente(clienteId.Value);
                return View(pedidos);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult DetalhePedido(int id)
        {
            try
            {
                int? clienteId = HttpContext.Session.GetInt32("ClienteId");
                if (clienteId == null)
                    return RedirectToAction("Login");

                var pedido = new PedidoDAO().Consulta(id);

                // cliente só pode ver os próprios pedidos
                if (pedido == null || pedido.ClienteId != clienteId.Value)
                    return RedirectToAction("MeusPedidos");

                pedido.Itens = new PedidoItemDAO().ListagemPorPedido(id);
                return View(pedido);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
    }
}
