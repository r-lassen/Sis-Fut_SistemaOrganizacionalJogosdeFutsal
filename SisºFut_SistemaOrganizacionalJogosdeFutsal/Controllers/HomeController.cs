using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Filters;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System.Diagnostics;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    [PaginaParaUsuarioLogado]
    public class HomeController : Controller
    {
        private readonly ISessao _sessao;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ITimeXQuadrasRepositorio _timeXquadrasRepositorio;
        private readonly IQuadrasRepositorio _quadrasRepositorio;

        public HomeController(IUsuarioRepositorio usuarioRepositorio,
            ISessao sessao,
            ITimeXQuadrasRepositorio timeXquadras,
            IQuadrasRepositorio quadras)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
            _timeXquadrasRepositorio = timeXquadras;
            _quadrasRepositorio = quadras;
        }



        public IActionResult Index()
        {
            HomeModel home = new HomeModel();

                home.Nome = "Gabriel Souza";
                home.Email = "Thelashgss@gmail.com";    

            return View(home);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Amistoso()
        {

            var Usuarios = _usuarioRepositorio.BuscarTodos();
            ViewBag.UserList = new SelectList(Usuarios, "Id", "Name");

            var UsuarioSessao = _sessao.BuscarSessaoDoUsuario();

            var TimeXQuadras = _timeXquadrasRepositorio.BuscarPorTime(UsuarioSessao.Id);

            var Quadras = _quadrasRepositorio.BuscarPorId(TimeXQuadras.Id);

            return View(new AgendamentosModel {Usuario = UsuarioSessao, UsuarioSelecionado = 0, Quadra = Quadras.NM_Quadra });
        }


    






    }
}
