using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using GameStoreMVC.Models;

namespace GameStoreMVC.DAO
{
    public class JogoDAO : PadraoDAO<JogoViewModel>
    {
        protected override DbParameter[] CriaParametros(JogoViewModel model)
        {
            DbParameter[] parametros =
            {
                HelperDAO.CriaParametro("id", model.Id),
                HelperDAO.CriaParametro("nome", model.Nome),
                HelperDAO.CriaParametro("descricao", model.Descricao),
                HelperDAO.CriaParametro("preco", model.Preco),
                HelperDAO.CriaParametro("estoque", model.Estoque),
                HelperDAO.CriaParametro("dataLancamento", model.DataLancamento),
                HelperDAO.CriaParametro("categoriaId", model.CategoriaId),
                HelperDAO.CriaParametro("plataformaId", model.PlataformaId),
                HelperDAO.CriaParametro("imagem", model.ImagemEmByte, DbType.Binary)
            };
            return parametros;
        }

        protected override JogoViewModel MontaModel(DataRow registro)
        {
            JogoViewModel j = new JogoViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                Nome = registro["nome"].ToString(),
                Descricao = registro["descricao"].ToString(),
                Preco = Convert.ToDecimal(registro["preco"]),
                Estoque = Convert.ToInt32(registro["estoque"]),
                DataLancamento = Convert.ToDateTime(registro["dataLancamento"]),
                CategoriaId = Convert.ToInt32(registro["categoriaId"]),
                PlataformaId = Convert.ToInt32(registro["plataformaId"])
            };

            if (registro["imagem"] != DBNull.Value)
                j.ImagemEmByte = registro["imagem"] as byte[];

            if (registro.Table.Columns.Contains("NomeCategoria"))
                j.NomeCategoria = registro["NomeCategoria"].ToString();

            if (registro.Table.Columns.Contains("NomePlataforma"))
                j.NomePlataforma = registro["NomePlataforma"].ToString();

            return j;
        }

        protected override void SetTabela()
        {
            Tabela = "Jogos";
        }

        // jogos da vitrine da área do cliente, com filtros
        public List<JogoViewModel> ListagemVitrine(string nome, int categoriaId, int plataformaId)
        {
            DbParameter[] p =
            {
                HelperDAO.CriaParametro("nome", nome ?? ""),
                HelperDAO.CriaParametro("categoriaId", categoriaId),
                HelperDAO.CriaParametro("plataformaId", plataformaId)
            };
            var tabela = HelperDAO.ExecutaProcSelect("spListagemVitrine", p);
            return MontaLista(tabela);
        }
    }
}
