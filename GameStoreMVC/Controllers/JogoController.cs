using System;
using System.Collections.Generic;
using System.IO;
using GameStoreMVC.DAO;
using GameStoreMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStoreMVC.Controllers
{
    public class JogoController : PadraoController<JogoViewModel>
    {
        public JogoController()
        {
            DAO = new JogoDAO();
            GeraProximoId = true;
        }

        protected override void PreencheDadosParaView(string Operacao, JogoViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);
            if (Operacao == "I" && model.DataLancamento == DateTime.MinValue)
                model.DataLancamento = DateTime.Today;
            PreparaComboCategorias();
            PreparaComboPlataformas();
        }

        private void PreparaComboCategorias()
        {
            var lista = new List<SelectListItem>();
            lista.Add(new SelectListItem("Selecione uma categoria...", "0"));
            foreach (var categoria in new CategoriaDAO().Listagem())
                lista.Add(new SelectListItem(categoria.Nome, categoria.Id.ToString()));
            ViewBag.Categorias = lista;
        }

        private void PreparaComboPlataformas()
        {
            var lista = new List<SelectListItem>();
            lista.Add(new SelectListItem("Selecione uma plataforma...", "0"));
            foreach (var plataforma in new PlataformaDAO().Listagem())
                lista.Add(new SelectListItem(plataforma.Nome, plataforma.Id.ToString()));
            ViewBag.Plataformas = lista;
        }

        // converte a imagem recebida no form em um vetor de bytes
        public byte[] ConvertImageToByte(IFormFile file)
        {
            if (file != null)
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    return ms.ToArray();
                }
            else
                return null;
        }

        protected override void ValidaDados(JogoViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);

            if (string.IsNullOrWhiteSpace(model.Nome))
                ModelState.AddModelError("Nome", "Preencha o nome.");
            if (model.Preco < 0)
                ModelState.AddModelError("Preco", "Preço inválido.");
            if (model.Estoque < 0)
                ModelState.AddModelError("Estoque", "Estoque não pode ser negativo.");
            if (model.CategoriaId <= 0)
                ModelState.AddModelError("CategoriaId", "Selecione uma categoria.");
            if (model.PlataformaId <= 0)
                ModelState.AddModelError("PlataformaId", "Selecione uma plataforma.");
            if (model.DataLancamento.Year < 1970)
                ModelState.AddModelError("DataLancamento", "Data inválida.");

            // imagem obrigatória só na inclusão
            if (model.Imagem == null && operacao == "I")
                ModelState.AddModelError("Imagem", "Escolha uma imagem.");
            if (model.Imagem != null && model.Imagem.Length / 1024 / 1024 >= 2)
                ModelState.AddModelError("Imagem", "Imagem limitada a 2 mb.");

            if (ModelState.IsValid)
            {
                // na alteração, sem imagem nova, mantém a que já estava salva
                if (operacao == "A" && model.Imagem == null)
                {
                    JogoViewModel jogo = DAO.Consulta(model.Id);
                    model.ImagemEmByte = jogo.ImagemEmByte;
                }
                else
                    model.ImagemEmByte = ConvertImageToByte(model.Imagem);
            }
        }
    }
}
