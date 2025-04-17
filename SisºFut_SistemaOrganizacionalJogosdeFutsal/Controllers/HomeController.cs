using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Filters;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System;
using System.Collections.Generic;
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
        private readonly IAgendamentoRepositorio _agendamentoRepositorio;

        public HomeController(IUsuarioRepositorio usuarioRepositorio,
            ISessao sessao,
            ITimeXQuadrasRepositorio timeXquadras,
            IQuadrasRepositorio quadras,
            IAgendamentoRepositorio agendamentos)

        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
            _timeXquadrasRepositorio = timeXquadras;
            _quadrasRepositorio = quadras;
            _agendamentoRepositorio = agendamentos;
        }



        //public IActionResult Index()
        //{
        //    HomeModel home = new HomeModel();

        //        home.Nome = "Gabriel Souza";
        //        home.Email = "Thelashgss@gmail.com";    

        //    return View(home);
        //}




        public IActionResult Index()
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Login", "Conta"); // Redireciona para login se não estiver logado
            }


            var abertos = _agendamentoRepositorio.ListarAbertos();

            var marcados = _agendamentoRepositorio.ListarMarcados();



            List<DadosAgendamentos> dadosAbertos = new List<DadosAgendamentos>();


            List<DadosAgendamentos> dadosMarcados = new List<DadosAgendamentos>();


            foreach (var aberto in abertos)
            {
                var time1 = _usuarioRepositorio.BuscarPorId(aberto.id_Time1);
                var quadra = _quadrasRepositorio.BuscarPorIdQuadra(aberto.id_Quadra);


                dadosAbertos.Add(new DadosAgendamentos { 
                    Time1 = time1.Name, 
                    Time2 = "", Local = quadra.NM_Quadra + "-" + quadra.DS_Endereco, 

                    Data = aberto.DT_Agendamento, DS_Descrição = aberto.DS_Descricao, 
                    Hora = aberto.HR_Agendamento,
                    id = aberto.Id,
                    FotoTime1 = time1.Foto
                });
            }


            foreach (var marcado in marcados)
            {
                var time1 = _usuarioRepositorio.BuscarPorId(marcado.id_Time1);
                var time2 = _usuarioRepositorio.BuscarPorId(marcado.id_Time2.Value);

                var quadra = _quadrasRepositorio.BuscarPorIdQuadra(marcado.id_Quadra);


                dadosMarcados.Add(new DadosAgendamentos
                {
                    Time1 = time1.Name,
                    Time2 = time2.Name,
                    Local = quadra.NM_Quadra + "-" + quadra.DS_Endereco,
                    Data = marcado.DT_Agendamento,
                    DS_Descrição = marcado.DS_Descricao,
                    Hora = marcado.HR_Agendamento,
                    id = marcado.Id,
                    FotoTime1 = time1.Foto,
                    FotoTime2 = time2.Foto
                });
            }



            HomeModel home = new HomeModel
            {
                Nome = usuarioLogado.Name,
                Email = usuarioLogado.Email,

                Abertos = dadosAbertos,
                Marcados = dadosMarcados,

                //Banco = usuarioLogado.Banco // Pegando o nome do banco (adicione essa propriedade ao modelo)
            };

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

            //var TimeXQuadras = _timeXquadrasRepositorio.BuscarPorTime(UsuarioSessao.Id);

            var Quadras = _quadrasRepositorio.BuscarPorId(UsuarioSessao.Id);

            //ViewBag.UsuarioSessao = UsuarioSessao;

            return View(new AgendamentosModel { Usuario = UsuarioSessao, id_Time2 = 0, Quadra = Quadras.NM_Quadra, id_Quadra = Quadras.Id, id_Time1 = UsuarioSessao.Id });

        }

        [HttpPost]
        public IActionResult Amistoso(AgendamentosModel agendamentos)
        {

            try
            {

                if (agendamentos.DT_Agendamento < DateTime.Now)
                {

                    var Usuarios = _usuarioRepositorio.BuscarTodos();

                    ViewBag.UserList = new SelectList(Usuarios, "Id", "Name");

                    TempData["MensagemErro"] = "Selecione um data Valida";
                    return View (agendamentos);
                }



                var Hora = agendamentos.HR_Agendamento.Split(":");

                if (Convert.ToInt32(Hora[0]) > 24 || Convert.ToInt32(Hora[1]) > 60)
                {
                    var Usuarios = _usuarioRepositorio.BuscarTodos();

                    ViewBag.UserList = new SelectList(Usuarios, "Id", "Name");

                    TempData["MensagemErro"] = "Selecione uma Hora Valida";
                    return View(agendamentos);
                }

                var resultado = _agendamentoRepositorio.Adicionar(agendamentos);
                TempData["MensagemSucesso"] = "Jogo cadastrado com Sucesso";

                return RedirectToAction("Index");




            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar seu Jogo, tente novamente. Detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }


        }



        public IActionResult JogoAberto()
        {

            var Usuarios = _usuarioRepositorio.BuscarTodos();

            ViewBag.UserList = new SelectList(Usuarios, "Id", "Name");

            var UsuarioSessao = _sessao.BuscarSessaoDoUsuario();

            //var TimeXQuadras = _timeXquadrasRepositorio.BuscarPorTime(UsuarioSessao.Id);

            var Quadras = _quadrasRepositorio.BuscarPorId(UsuarioSessao.Id);

            //ViewBag.UsuarioSessao = UsuarioSessao;

            return View(new AgendamentosModel { Usuario = UsuarioSessao, id_Time2 = 0, Quadra = Quadras.NM_Quadra, id_Quadra = Quadras.Id, id_Time1 = UsuarioSessao.Id });

        }



        public IActionResult JogoMarcado(int id)
        {

            var Agendamento = _agendamentoRepositorio.BuscarPorId(id);


            var time1 = _usuarioRepositorio.BuscarPorId(Agendamento.id_Time1);
            var time2 = _usuarioRepositorio.BuscarPorId(Agendamento.id_Time2.Value);

            var quadra = _quadrasRepositorio.BuscarPorIdQuadra(Agendamento.id_Quadra);

            var UsuarioSessao = _sessao.BuscarSessaoDoUsuario();

            DadosAgendamentos dadosMarcados = new DadosAgendamentos()

            {
                Time1 = time1.Name,
                Time2 = time2.Name,
                Local = quadra.NM_Quadra + "-" + quadra.DS_Endereco,
                Data = Agendamento.DT_Agendamento,
                DS_Descrição = Agendamento.DS_Descricao,
                Hora = Agendamento.HR_Agendamento,
                id = Agendamento.Id,
                FotoTime1 = time1.Foto,
                FotoTime2 = time2.Foto,
                idTime1 = time1.Id,
                idTime2 = time2.Id,
            };

            return View(new AgendamentosModel {
                UsuarioId = UsuarioSessao.Id,
                Agendar = dadosMarcados
            });

        }


    }
}
