using System;
using System.Linq;
using System.Transactions;
using GameStoreMVC.DAO;
using GameStoreMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreMVC.Controllers
{
    // carrinho de compras e fechamento do pedido (funcionalidade principal)
    public class CarrinhoController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var carrinho = HelperControllers.ObtemCarrinhoNaSession(HttpContext.Session);
                JogoDAO dao = new JogoDAO();
                foreach (var item in carrinho)
                    item.ImagemSrc = dao.Consulta(item.JogoId)?.ImagemSrc;

                ViewBag.Total = carrinho.Sum(c => c.Subtotal);
                return View(carrinho);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        // AJAX: adiciona / altera a quantidade de um jogo no carrinho
        [HttpPost]
        public IActionResult AdicionarCarrinho(int jogoId, int quantidade, bool somar)
        {
            try
            {
                var carrinho = HelperControllers.ObtemCarrinhoNaSession(HttpContext.Session);
                var item = carrinho.Find(c => c.JogoId == jogoId);
                var jogo = new JogoDAO().Consulta(jogoId);

                if (jogo == null)
                    return Json(new { erro = true, msg = "Jogo não encontrado." });

                int novaQtde = somar && item != null ? item.Quantidade + quantidade : quantidade;

                if (novaQtde > jogo.Estoque)
                    return Json(new { erro = true, msg = $"Temos apenas {jogo.Estoque} unidade(s) de {jogo.Nome} em estoque." });

                if (novaQtde <= 0)
                {
                    // quantidade zero tira do carrinho
                    if (item != null)
                        carrinho.Remove(item);
                }
                else
                {
                    if (item == null)
                    {
                        item = new ItemCarrinhoViewModel { JogoId = jogo.Id };
                        carrinho.Add(item);
                    }
                    item.Nome = jogo.Nome;
                    item.Preco = jogo.Preco;
                    item.Quantidade = novaQtde;
                }

                HelperControllers.SalvaCarrinhoNaSession(HttpContext.Session, carrinho);

                return Json(new
                {
                    erro = false,
                    msg = $"{jogo.Nome} atualizado no carrinho.",
                    qtdeItens = carrinho.Sum(c => c.Quantidade),
                    subtotal = (item?.Subtotal ?? 0).ToString("C"),
                    total = carrinho.Sum(c => c.Subtotal).ToString("C")
                });
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        public IActionResult Remover(int jogoId)
        {
            var carrinho = HelperControllers.ObtemCarrinhoNaSession(HttpContext.Session);
            carrinho.RemoveAll(c => c.JogoId == jogoId);
            HelperControllers.SalvaCarrinhoNaSession(HttpContext.Session, carrinho);
            return RedirectToAction("Index");
        }

        // grava o pedido (mestre) e os itens (detalhe) dentro de uma transação
        [HttpPost]
        public IActionResult EfetuarPedido()
        {
            try
            {
                int? clienteId = HttpContext.Session.GetInt32("ClienteId");
                if (clienteId == null)
                    return RedirectToAction("Login", "Conta", new { retorno = "carrinho" });

                var carrinho = HelperControllers.ObtemCarrinhoNaSession(HttpContext.Session);
                if (carrinho.Count == 0)
                    return RedirectToAction("Index");

                // confere estoque e usa o preço atual do banco
                JogoDAO jogoDAO = new JogoDAO();
                foreach (var item in carrinho)
                {
                    var jogo = jogoDAO.Consulta(item.JogoId);
                    if (jogo == null || jogo.Estoque < item.Quantidade)
                    {
                        TempData["Erro"] = $"Estoque insuficiente para {item.Nome}. Ajuste a quantidade.";
                        return RedirectToAction("Index");
                    }
                    item.Preco = jogo.Preco;
                }

                int idPedido;
                using (var transacao = new TransactionScope())
                {
                    PedidoViewModel pedido = new PedidoViewModel
                    {
                        Data = DateTime.Now,
                        ClienteId = clienteId.Value,
                        ValorTotal = carrinho.Sum(c => c.Subtotal),
                        Status = "Aguardando pagamento"
                    };
                    idPedido = new PedidoDAO().Insert(pedido);

                    PedidoItemDAO itemDAO = new PedidoItemDAO();
                    foreach (var elemento in carrinho)
                    {
                        PedidoItemViewModel item = new PedidoItemViewModel
                        {
                            PedidoId = idPedido,
                            JogoId = elemento.JogoId,
                            Qtde = elemento.Quantidade,
                            ValorUnitario = elemento.Preco
                        };
                        itemDAO.Insert(item);
                    }
                    transacao.Complete();
                }

                HelperControllers.LimparCarrinho(HttpContext.Session);
                TempData["Mensagem"] = $"Pedido nº {idPedido} realizado com sucesso!";
                return RedirectToAction("DetalhePedido", "Conta", new { id = idPedido });
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
    }
}
