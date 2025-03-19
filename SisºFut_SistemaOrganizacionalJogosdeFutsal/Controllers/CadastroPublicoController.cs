using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;

using System;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    public class CadastroPublicoController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ITimeXQuadrasRepositorio _timeXquadrasRepositorio;
        private readonly IQuadrasRepositorio _quadrasRepositorio;

        public CadastroPublicoController(IUsuarioRepositorio usuarioRepositorio,


            ITimeXQuadrasRepositorio timeXquadras,
            IQuadrasRepositorio quadras)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _timeXquadrasRepositorio = timeXquadras;
            _quadrasRepositorio = quadras; 
        }

        [HttpGet]
        [AllowAnonymous] // Permite acesso sem login
        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Cadastro(CadastroModel cadastro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cadastro.usuario.Perfil = Enums.PerfilEnum.Padrao; // Define um perfil padrão
                    _usuarioRepositorio.Adicionar(cadastro.usuario);


                    QuadrasModel quadras = new QuadrasModel { DS_Endereco = cadastro.DS_Endereco, NM_Quadra = cadastro.NM_Quadra };
                    _quadrasRepositorio.Adicionar(quadras);
                    _timeXquadrasRepositorio.Adicionar(new TimeXQuadrasModel { id_Quadra = quadras.Id, id_Time = cadastro.usuario.Id }); 

                    TempData["MensagemSucesso"] = "Cadastro realizado com sucesso!";
                    return RedirectToAction("Index", "Login"); // Redireciona para login
                }

                return View(cadastro.usuario);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar usuário. Detalhe: {erro.Message}";
                return View(cadastro.usuario);
            }
        }








    }
}
