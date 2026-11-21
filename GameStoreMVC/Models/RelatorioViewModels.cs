namespace GameStoreMVC.Models
{
    // retorno da API de vendas por categoria
    public class VendaCategoriaViewModel
    {
        public string Categoria { get; set; }
        public int QtdeVendida { get; set; }
        public decimal ValorTotal { get; set; }
    }

    // retorno da API de jogos mais vendidos
    public class JogoMaisVendidoViewModel
    {
        public int JogoId { get; set; }
        public string Jogo { get; set; }
        public string Plataforma { get; set; }
        public int QtdeVendida { get; set; }
        public decimal ValorTotal { get; set; }
    }

    // retorno da API de resumo geral
    public class ResumoVendasViewModel
    {
        public int TotalPedidos { get; set; }
        public decimal Faturamento { get; set; }
        public decimal TicketMedio { get; set; }
        public int TotalClientes { get; set; }
        public int TotalJogos { get; set; }
    }
}
