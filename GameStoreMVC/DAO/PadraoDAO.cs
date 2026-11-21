using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using GameStoreMVC.Models;

namespace GameStoreMVC.DAO
{
    public abstract class PadraoDAO<T> where T : PadraoViewModel
    {
        public PadraoDAO()
        {
            SetTabela();
        }

        protected string Tabela { get; set; }
        protected bool ChaveIdentity { get; set; } = false;
        protected abstract DbParameter[] CriaParametros(T model);
        protected abstract T MontaModel(DataRow registro);
        protected abstract void SetTabela();

        public virtual int Insert(T model)
        {
            return HelperDAO.ExecutaProc("spInsert_" + Tabela, CriaParametros(model), ChaveIdentity);
        }

        public virtual void Update(T model)
        {
            HelperDAO.ExecutaProc("spUpdate_" + Tabela, CriaParametros(model));
        }

        public virtual void Delete(int id)
        {
            var p = new DbParameter[]
            {
                HelperDAO.CriaParametro("id", id),
                HelperDAO.CriaParametro("tabela", Tabela)
            };
            HelperDAO.ExecutaProc("spDelete", p);
        }

        public virtual T Consulta(int id)
        {
            var p = new DbParameter[] { HelperDAO.CriaParametro("id", id) };
            var tabela = HelperDAO.ExecutaProcSelect("spConsulta_" + Tabela, p);
            if (tabela.Rows.Count == 0)
                return null;
            else
                return MontaModel(tabela.Rows[0]);
        }

        public virtual int ProximoId()
        {
            var p = new DbParameter[] { HelperDAO.CriaParametro("tabela", Tabela) };
            var tabela = HelperDAO.ExecutaProcSelect("spProximoId", p);
            return Convert.ToInt32(tabela.Rows[0][0]);
        }

        // a listagem já vem com o join das tabelas relacionadas
        public virtual List<T> Listagem()
        {
            var tabela = HelperDAO.ExecutaProcSelect("spListagem_" + Tabela, null);
            return MontaLista(tabela);
        }

        protected List<T> MontaLista(DataTable tabela)
        {
            List<T> lista = new List<T>();
            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModel(registro));
            return lista;
        }
    }
}
