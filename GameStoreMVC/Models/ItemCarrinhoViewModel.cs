using Newtonsoft.Json;

namespace GameStoreMVC.Models
{
    // item guardado na session do carrinho
    public class ItemCarrinhoViewModel
    {
        public int JogoId { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }

        // a imagem não vai para a session (fica muito grande), é buscada no banco ao exibir
        [JsonIgnore]
        public string ImagemSrc { get; set; }

        public decimal Subtotal => Quantidade * Preco;
    }
}
