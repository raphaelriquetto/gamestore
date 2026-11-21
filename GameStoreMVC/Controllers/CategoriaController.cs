using GameStoreMVC.DAO;
using GameStoreMVC.Models;

namespace GameStoreMVC.Controllers
{
    public class CategoriaController : PadraoController<CategoriaViewModel>
    {
        public CategoriaController()
        {
            DAO = new CategoriaDAO();
            GeraProximoId = true;
        }

        protected override void ValidaDados(CategoriaViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);
            if (string.IsNullOrWhiteSpace(model.Nome))
                ModelState.AddModelError("Nome", "Preencha o nome.");
            else if (model.Nome.Length > 50)
                ModelState.AddModelError("Nome", "Máximo de 50 caracteres.");
        }
    }
}
