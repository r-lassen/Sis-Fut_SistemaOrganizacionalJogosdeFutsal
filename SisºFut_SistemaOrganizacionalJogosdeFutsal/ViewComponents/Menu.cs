//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
//using System.Threading.Tasks;
//namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.ViewComponents
//{
//    public class Menu : ViewComponent
//    {
//        public async Task<IViewComponentResult> InvokeAsync()
//        {
//            string sessaoUsuario = HttpContext.Session.GetString("sessaoUsuarioLogado");
//            if (string.IsNullOrEmpty(sessaoUsuario)) return null;
//            UsuarioModel usuario = JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);
//            return View(usuario);
//        }
//    }
//}


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Threading.Tasks;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.ViewComponents
{
    public class Menu : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string sessaoUsuario = HttpContext.Session.GetString("sessaoUsuarioLogado");

            if (string.IsNullOrEmpty(sessaoUsuario))
                return View(new UsuarioModel()); // Retorna um modelo vazio

            UsuarioModel usuario = JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);

            return View(usuario);
        }
    }
}
