using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    public class MeuPerfilController : Controller
    {


        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISessao _sessao;
        public MeuPerfilController(IUsuarioRepositorio usuarioRepositorio,
                                      ISessao sessao)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
        }

        public IActionResult MeuPerfil()
        {
            return View();
        }

        //[HttpPost]
        //public iactionresult alterar(alterarsenhamodel alterarsenhamodel)
        //{
        //    try
        //    {
        //        usuariomodel usuariologado = _sessao.buscarsessaodousuario();
        //        alterarsenhamodel.id = usuariologado.id;

        //        if (modelstate.isvalid)
        //        {
        //            _usuariorepositorio.alterarsenha(alterarsenhamodel);
        //            tempdata["mensagemsucesso"] = "senha alterada com sucesso!";
        //            return view("index", alterarsenhamodel);
        //        }

        //        return view("index", alterarsenhamodel);
        //    }
        //    catch (exception erro)
        //    {
        //        tempdata["mensagemerro"] = $"ops, não conseguimos alterar sua senha, tente novamante, detalhe do erro: {erro.message}";
        //        return view("index", alterarsenhamodel);
        //    }
        //}


    }
}
