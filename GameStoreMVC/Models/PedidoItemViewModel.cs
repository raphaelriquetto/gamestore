using System;

namespace GameStoreMVC.Models
{
    public class PedidoItemViewModel : PadraoViewModel
    {
        public int PedidoId { get; set; }
        public int JogoId { get; set; }
        public int Qtde { get; set; }
        public decimal ValorUnitario { get; set; }

        public decimal Subtotal => Qtde * ValorUnitario;

        // campos vindos do join
        public string NomeJogo { get; set; }
        public string NomeCategoria { get; set; }
        public string NomePlataforma { get; set; }
        public string NomeCliente { get; set; }
        public DateTime DataPedido { get; set; }
    }
}
