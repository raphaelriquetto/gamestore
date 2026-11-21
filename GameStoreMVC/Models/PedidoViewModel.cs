using System;
using System.Collections.Generic;

namespace GameStoreMVC.Models
{
    public class PedidoViewModel : PadraoViewModel
    {
        public DateTime Data { get; set; }
        public int ClienteId { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; }

        // campos vindos do join
        public string NomeCliente { get; set; }
        public int QtdeItens { get; set; }

        // itens do pedido (detalhe)
        public List<PedidoItemViewModel> Itens { get; set; } = new List<PedidoItemViewModel>();

        // status possíveis de um pedido
        public static readonly string[] ListaStatus = { "Aguardando pagamento", "Pago", "Enviado", "Entregue", "Cancelado" };
    }
}
