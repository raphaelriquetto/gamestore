using System;
using GameStoreMVC.DAO;
using GameStoreMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreMVC.Controllers
{
    // login da área administrativa
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FazLogin(string usuario, string senha)
        {
            try
            {
                string nome = null;
                if (!string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(senha))
                    nome = new AdministradorDAO().Login(usuario, HelperControllers.HashSenha(senha));

                if (nome != null)
                {
                    HttpContext.Session.SetString("Logado", "true");
                    HttpContext.Session.SetString("NomeAdmin", nome);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.Erro = "Usuário ou senha inválidos!";
                    return View("Index");
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult LogOff()
        {
            HttpContext.Session.Remove("Logado");
            HttpContext.Session.Remove("NomeAdmin");
            return RedirectToAction("Index");
        }
    }
}
