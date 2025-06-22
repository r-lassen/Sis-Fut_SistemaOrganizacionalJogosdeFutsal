using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System;
using System.Collections.Generic;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    public class JogosEncerradosController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IJogosEncerradosRepositorio _jogosEncerradosRepositorio;
        private readonly IQuadrasRepositorio _quadrasRepositorio;
        private readonly ISessao _sessao;
        public JogosEncerradosController
            (IUsuarioRepositorio usuarioRepositorio,
            ISessao sessao,
            IJogosEncerradosRepositorio jogosEncerradosRepositorio,
            IQuadrasRepositorio quadrasRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
            _jogosEncerradosRepositorio = jogosEncerradosRepositorio;
            _quadrasRepositorio = quadrasRepositorio;
        }



        public IActionResult Index()
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Login", "Conta");
            }

            var jogos = _jogosEncerradosRepositorio.BuscarTodos();
            var dadosEncerrados = new List<DadosJogosEncerrados>();

            foreach (var jogo in jogos)
            {
                var time1 = _usuarioRepositorio.BuscarPorId(jogo.IdTime1);
                var time2 = _usuarioRepositorio.BuscarPorId(jogo.IdTime2);
                var quadra = _quadrasRepositorio.BuscarPorIdQuadra(jogo.IdQuadra);

                if (time1 == null || time2 == null || quadra == null)
                    continue; // previne erros caso algum dado esteja faltando

                dadosEncerrados.Add(new DadosJogosEncerrados
                {
                    id = jogo.Id,
                    idTime1 = time1.Id,
                    idTime2 = time2.Id,
                    Time1 = time1.Name,
                    Time2 = time2.Name,
                    GolsTime1 = jogo.GolsTime1,
                    GolsTime2 = jogo.GolsTime2,
                    FotoTime1 = time1.Foto,
                    FotoTime2 = time2.Foto,
                    Data = jogo.DataEncerramento,
                    //Hora = jogo.DataEncerramento.ToString("HH:mm"),
                    DS_Descrição = jogo.Descricao,
                    Local = $"{quadra.NM_Quadra} - {quadra.DS_Endereco}"
                });
            }

            var home = new HomeModel
            {
                Id = usuarioLogado.Id,
                Nome = usuarioLogado.Name,
                Email = usuarioLogado.Email,
                Encerrados = dadosEncerrados
            };

            return View(home);
        }





    }
}
