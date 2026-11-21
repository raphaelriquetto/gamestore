using GameStoreMVC.DAO;
using GameStoreMVC.Models;

namespace GameStoreMVC.Controllers
{
    public class PlataformaController : PadraoController<PlataformaViewModel>
    {
        public PlataformaController()
        {
            DAO = new PlataformaDAO();
            GeraProximoId = true;
        }

        protected override void ValidaDados(PlataformaViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);
            if (string.IsNullOrWhiteSpace(model.Nome))
                ModelState.AddModelError("Nome", "Preencha o nome.");
            if (string.IsNullOrWhiteSpace(model.Fabricante))
                ModelState.AddModelError("Fabricante", "Preencha o fabricante.");
        }
    }
}
