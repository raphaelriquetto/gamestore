using System;
using Microsoft.AspNetCore.Http;

namespace GameStoreMVC.Models
{
    public class JogoViewModel : PadraoViewModel
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public DateTime DataLancamento { get; set; }
        public int CategoriaId { get; set; }
        public int PlataformaId { get; set; }

        // campos preenchidos pelo join (não são salvos na tabela jogos)
        public string NomeCategoria { get; set; }
        public string NomePlataforma { get; set; }

        // imagem recebida do form
        public IFormFile Imagem { get; set; }

        // imagem em bytes para salvar no BD
        public byte[] ImagemEmByte { get; set; }

        // imagem pronta para exibir na tela
        public string ImagemEmBase64
        {
            get
            {
                if (ImagemEmByte != null)
                    return Convert.ToBase64String(ImagemEmByte);
                else
                    return string.Empty;
            }
        }

        // src da tag img (usa uma imagem padrão quando o jogo não tem foto)
        public string ImagemSrc
        {
            get
            {
                if (ImagemEmByte != null)
                    return "data:image/jpeg;base64," + ImagemEmBase64;
                else
                    return "/images/sem-imagem.svg";
            }
        }
    }
}
