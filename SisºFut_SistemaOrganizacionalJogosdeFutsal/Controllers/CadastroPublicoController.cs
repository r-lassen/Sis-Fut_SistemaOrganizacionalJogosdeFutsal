using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;

using System;
using System.IO;

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

                    // Verificar se já existe o mesmo e-mail
                    var emailExiste = _usuarioRepositorio.BuscarPorEmail(cadastro.usuario.Email);
                    if (emailExiste != null)
                    {
                        ModelState.AddModelError("usuario.Email", "Este e-mail já está cadastrado.");
                    }

                    // Verificar se já existe o mesmo login
                    var loginExiste = _usuarioRepositorio.BuscarPorLogin(cadastro.usuario.Login);
                    if (loginExiste != null)
                    {
                        ModelState.AddModelError("usuario.Login", "Este login já está em uso.");
                    }

                    // Verificar se já existe o mesmo nome de time (usando login como base)
                    var nomeTimeExiste = _usuarioRepositorio.BuscarPorNomeTime(cadastro.usuario.Name);
                    if (nomeTimeExiste != null)
                    {
                        ModelState.AddModelError("usuario.Name", "Este nome de time já está em uso.");
                    }

                    // Se alguma validação falhar, retorna a View
                    if (!ModelState.IsValid)
                    {
                        return View(cadastro);
                    }

                    // Continua com o cadastro se não houver erros
                    if (!string.IsNullOrEmpty(cadastro.usuario.Login) && !string.IsNullOrEmpty(cadastro.usuario.Senha))
                    {
                        var base64 = "";

                        if (cadastro.usuario.FotoArquivo != null && cadastro.usuario.FotoArquivo.Length > 0)
                        {
                            base64 = ConverterParaBase64(cadastro.usuario.FotoArquivo);
                            cadastro.usuario.Foto = base64;
                        }

                        cadastro.usuario.Perfil = Enums.PerfilEnum.Padrao;
                        var a = _usuarioRepositorio.Adicionar(cadastro.usuario);

                        QuadrasModel quadras = new QuadrasModel
                        {
                            DS_Endereco = cadastro.DS_Endereco ?? string.Empty,
                            NM_Quadra = cadastro.NM_Quadra ?? string.Empty,
                            id_Time = a.Id
                        };
                        _quadrasRepositorio.Adicionar(quadras);

                        TempData["MensagemSucesso"] = "Cadastro realizado com sucesso!";
                        return RedirectToAction("Index", "Login");
                    }


                return View(cadastro);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar usuário. Detalhe: {erro.Message}";
                return View(cadastro);
            }
        }





        public string ConverterParaBase64(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                arquivo.CopyTo(memoryStream); // versão síncrona
                var bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }




    }
}
