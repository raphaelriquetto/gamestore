using System;
using System.Collections.Generic;
using GameStoreMVC.DAO;
using GameStoreMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreMVC.Controllers.Api
{
    /// <summary>
    /// APIs com dados consolidados das vendas (pedidos cancelados não entram nos totais)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RelatoriosController : ControllerBase
    {
        /// <summary>
        /// Quantidade vendida e faturamento agrupados por categoria de jogo.
        /// </summary>
        /// <param name="dataInicial">Data inicial (opcional, formato yyyy-MM-dd)</param>
        /// <param name="dataFinal">Data final (opcional, formato yyyy-MM-dd)</param>
        /// <response code="200">Lista de categorias com os totais vendidos</response>
        /// <response code="400">Período inválido</response>
        [HttpGet("vendas-por-categoria")]
        [ProducesResponseType(typeof(List<VendaCategoriaViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult VendasPorCategoria(DateTime? dataInicial, DateTime? dataFinal)
        {
            try
            {
                DateTime inicio = dataInicial ?? new DateTime(1900, 1, 1);
                DateTime fim = dataFinal?.Date.AddDays(1).AddSeconds(-1) ?? new DateTime(9999, 12, 31);
                if (inicio > fim)
                    return BadRequest(new { erro = "A data inicial deve ser menor que a data final." });

                return Ok(new RelatorioDAO().VendasPorCategoria(inicio, fim));
            }
            catch (Exception erro)
            {
                return StatusCode(500, new { erro = erro.Message });
            }
        }

        /// <summary>
        /// Ranking dos jogos mais vendidos.
        /// </summary>
        /// <param name="top">Quantidade de jogos no ranking (1 a 50, padrão 5)</param>
        /// <response code="200">Ranking dos jogos</response>
        /// <response code="400">Parâmetro top inválido</response>
        [HttpGet("jogos-mais-vendidos")]
        [ProducesResponseType(typeof(List<JogoMaisVendidoViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult JogosMaisVendidos(int top = 5)
        {
            try
            {
                if (top < 1 || top > 50)
                    return BadRequest(new { erro = "O parâmetro top deve estar entre 1 e 50." });

                return Ok(new RelatorioDAO().JogosMaisVendidos(top));
            }
            catch (Exception erro)
            {
                return StatusCode(500, new { erro = erro.Message });
            }
        }

        /// <summary>
        /// Resumo geral da loja: total de pedidos, faturamento, ticket médio, clientes e jogos.
        /// </summary>
        /// <response code="200">Resumo das vendas</response>
        [HttpGet("resumo")]
        [ProducesResponseType(typeof(ResumoVendasViewModel), StatusCodes.Status200OK)]
        public IActionResult Resumo()
        {
            try
            {
                return Ok(new RelatorioDAO().Resumo());
            }
            catch (Exception erro)
            {
                return StatusCode(500, new { erro = erro.Message });
            }
        }
    }
}
