using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GameStoreMVC.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace GameStoreMVC.Controllers
{
    public class HelperControllers
    {
        // verifica se o administrador está logado
        public static bool VerificaUserLogado(ISession session)
        {
            return session.GetString("Logado") != null;
        }

        // verifica se o cliente está logado
        public static bool VerificaClienteLogado(ISession session)
        {
            return session.GetInt32("ClienteId") != null;
        }

        // gera o hash SHA-256 da senha
        public static string HashSenha(string senha)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }

        // carrinho fica guardado na session no formato JSON
        public static List<ItemCarrinhoViewModel> ObtemCarrinhoNaSession(ISession session)
        {
            var carrinho = new List<ItemCarrinhoViewModel>();
            string carrinhoJson = session.GetString("carrinho");
            if (carrinhoJson != null)
                carrinho = JsonConvert.DeserializeObject<List<ItemCarrinhoViewModel>>(carrinhoJson);
            return carrinho;
        }

        public static void SalvaCarrinhoNaSession(ISession session, List<ItemCarrinhoViewModel> carrinho)
        {
            session.SetString("carrinho", JsonConvert.SerializeObject(carrinho));
        }

        public static void LimparCarrinho(ISession session)
        {
            session.Remove("carrinho");
        }

        public static int QtdeItensCarrinho(ISession session)
        {
            return ObtemCarrinhoNaSession(session).Sum(c => c.Quantidade);
        }
    }
}
