using GameStoreMVC.DAO;
using GameStoreMVC.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GameStoreMVC.Controllers
{
    public class ClienteController : PadraoController<ClienteViewModel>
    {
        public ClienteController()
        {
            DAO = new ClienteDAO();
            GeraProximoId = true;
        }

        protected override void PreencheDadosParaView(string Operacao, ClienteViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);
            model.Senha = null; // nunca devolve o hash para a tela
        }

        protected override void ValidaDados(ClienteViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);
            ValidaCampos(model, operacao, ModelState, (ClienteDAO)DAO);
        }

        // validação usada aqui e também no cadastro feito pelo próprio cliente
        public static void ValidaCampos(ClienteViewModel model, string operacao, ModelStateDictionary modelState, ClienteDAO dao)
        {
            if (string.IsNullOrWhiteSpace(model.Nome))
                modelState.AddModelError("Nome", "Preencha o nome.");

            if (string.IsNullOrWhiteSpace(model.Email) || !model.Email.Contains("@"))
                modelState.AddModelError("Email", "Informe um e-mail válido.");
            else
            {
                var outro = dao.ConsultaPorEmail(model.Email);
                if (outro != null && outro.Id != model.Id)
                    modelState.AddModelError("Email", "Este e-mail já está cadastrado.");
            }

            if (string.IsNullOrWhiteSpace(model.Cpf) || model.Cpf.Length < 11)
                modelState.AddModelError("Cpf", "Informe o CPF.");

            if (string.IsNullOrWhiteSpace(model.Cep))
                modelState.AddModelError("Cep", "Informe o CEP.");

            if (string.IsNullOrWhiteSpace(model.Uf) || model.Uf.Length != 2)
                modelState.AddModelError("Uf", "UF inválida.");

            // senha obrigatória na inclusão; na alteração, em branco mantém a atual
            if (operacao == "I" && (string.IsNullOrEmpty(model.Senha) || model.Senha.Length < 4))
                modelState.AddModelError("Senha", "A senha deve ter ao menos 4 caracteres.");

            if (modelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Senha))
                    model.Senha = dao.Consulta(model.Id).Senha;
                else
                    model.Senha = HelperControllers.HashSenha(model.Senha);
            }
        }
    }
}
