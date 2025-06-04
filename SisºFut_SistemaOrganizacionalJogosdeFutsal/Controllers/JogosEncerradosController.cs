using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    public class JogosEncerradosController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IJogosEncerradosRepositorio _jogosEncerradosRepositorio;
        private readonly ISessao _sessao;
        public JogosEncerradosController
            (IUsuarioRepositorio usuarioRepositorio,
            ISessao sessao,
            IJogosEncerradosRepositorio jogosEncerradosRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
            _jogosEncerradosRepositorio = jogosEncerradosRepositorio;
        }









    }
}
