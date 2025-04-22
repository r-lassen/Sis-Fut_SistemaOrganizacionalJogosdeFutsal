using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Data;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Filters;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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




        //---------------Essa carrega a view--------------
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


                dadosAbertos.Add(new DadosAgendamentos
                {
                    Time1 = time1.Name,
                    Time2 = "",
                    Local = quadra.NM_Quadra + "-" + quadra.DS_Endereco,
                    Data = aberto.DT_Agendamento,
                    DS_Descrição = aberto.DS_Descricao,
                    Hora = aberto.HR_Agendamento,
                    id = aberto.Id,
                    FotoTime1 = time1.Foto,
                    idTime1 = time1.Id,

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
                    FotoTime2 = time2.Foto,
                    idTime1 = time1.Id,
                    idTime2 = time2.Id,
                });
            }





            HomeModel home = new HomeModel
            {
                Nome = usuarioLogado.Name,
                Email = usuarioLogado.Email,
                Id = usuarioLogado.Id,

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


        //---------------Essa carrega a view--------------
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
                    return View(agendamentos);
                }



                var Hora = agendamentos.HR_Agendamento.Split(":");

                if (Convert.ToInt32(Hora[0]) > 24 || Convert.ToInt32(Hora[1]) > 60)
                {
                    var Usuarios = _usuarioRepositorio.BuscarTodos();

                    ViewBag.UserList = new SelectList(Usuarios, "Id", "Name");

                    TempData["MensagemErro"] = "Selecione uma Hora Valida";
                    return View(agendamentos);
                }

                var jaTemAgendadoTime1 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime1(agendamentos.id_Time1);
                var jaExisteData = false;


                foreach (var a in jaTemAgendadoTime1)
                {
                    if (a.DT_Agendamento == agendamentos.DT_Agendamento)
                    {
                        jaExisteData = true;

                    }
                }

                if (agendamentos.id_Time2 > 0)
                {
                    var jaTemAgendadoTime2 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime2((int)agendamentos.id_Time2);
                    foreach (var a in jaTemAgendadoTime2)
                    {
                        if (a.DT_Agendamento == agendamentos.DT_Agendamento)
                        {
                            jaExisteData = true;

                        }
                    }

                }

                if (!jaExisteData)
                {
                    var resultado = _agendamentoRepositorio.Adicionar(agendamentos);
                    TempData["MensagemSucesso"] = "Jogo cadastrado com Sucesso";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Já existe jogo marcado para essa data";
                    return RedirectToAction("Amistoso");
                }



            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar seu Jogo, tente novamente. Detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }


        }



        //---------------Essa carrega a view--------------
        public IActionResult JogoAberto(int id)
        {
            var Agendamento = _agendamentoRepositorio.BuscarPorId(id);

            var UsuarioSessao = _sessao.BuscarSessaoDoUsuario();

            var time1 = _usuarioRepositorio.BuscarPorId(Agendamento.id_Time1);
            var time2 = _usuarioRepositorio.BuscarPorId(UsuarioSessao.Id);

            var quadra = _quadrasRepositorio.BuscarPorIdQuadra(Agendamento.id_Quadra);

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


            return View(new AgendamentosModel
            {
                UsuarioId = UsuarioSessao.Id,
                Agendar = dadosMarcados
            });

        }

        [HttpPost]
        public IActionResult JogoAberto(AgendamentosModel agendamentos)
        {
            try
            {
                var Agendado = _agendamentoRepositorio.BuscarPorId(agendamentos.Agendar.id);

                if (Agendado != null)
                {
                    var jaTemAgendadoTime1 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime1(agendamentos.Agendar.idTime1);
                    var jaExisteData = false;
                    var jaTemAgendadoTime1MenosoAtual = jaTemAgendadoTime1.Where(m => m.Id != Agendado.Id);

                    foreach (var a in jaTemAgendadoTime1MenosoAtual)
                    {
                        if (a.DT_Agendamento == agendamentos.Agendar.Data)
                        {
                            jaExisteData = true;

                        }
                    }

                    if (agendamentos.Agendar.idTime2 > 0)
                    {
                        var jaTemAgendadoTime2 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime2(agendamentos.Agendar.idTime2);
                        foreach (var a in jaTemAgendadoTime2)
                        {
                            if (a.DT_Agendamento == agendamentos.Agendar.Data)
                            {
                                jaExisteData = true;
                            }
                        }
                    }

                    if (!jaExisteData)
                    {
                        Agendado.id_Time2 = agendamentos.Agendar.idTime2;
                        Agendado.DT_Atualizacao = DateTime.Now;

                        var resultado = _agendamentoRepositorio.Atualizar(Agendado);
                        TempData["MensagemSucesso"] = "Jogo marcado com Sucesso";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["MensagemErro"] = "JÃ¡ existe jogo marcado para essa data";
                        return RedirectToAction("Index");
                    }


                }
                else
                {

                    TempData["MensagemErro"] = "Erro ao marcar o jogo, tente novamente";
                    return RedirectToAction("Index");
                }






            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar seu Jogo, tente novamente. Detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }


        }

        //---------------Essa carrega a view--------------
        public IActionResult EditarJogoAberto(int id)
        {
            var Agendamento = _agendamentoRepositorio.BuscarPorId(id);

            var time1 = _usuarioRepositorio.BuscarPorId(Agendamento.id_Time1);
            // var time2 = _usuarioRepositorio.BuscarPorId(UsuarioSessao.Id);

            var quadra = _quadrasRepositorio.BuscarPorIdQuadra(Agendamento.id_Quadra);

            DadosAgendamentos dadosAbertos = new DadosAgendamentos()
            {
                Time1 = time1.Name,
                //Time2 = time2.Name,
                Local = quadra.NM_Quadra + "-" + quadra.DS_Endereco,
                Data = Agendamento.DT_Agendamento,
                DS_Descrição = Agendamento.DS_Descricao,
                Hora = Agendamento.HR_Agendamento,
                id = Agendamento.Id,
                FotoTime1 = time1.Foto,
                // FotoTime2 = time2.Foto,
                idTime1 = time1.Id,
                // idTime2 = time2.Id,
            };


            var Usuarios = _usuarioRepositorio.BuscarTodos();

            ViewBag.UserList = new SelectList(Usuarios, "Id", "Name");

            var UsuarioSessao = _sessao.BuscarSessaoDoUsuario();

            //var TimeXQuadras = _timeXquadrasRepositorio.BuscarPorTime(UsuarioSessao.Id);

            var Quadras = _quadrasRepositorio.BuscarPorId(UsuarioSessao.Id);

            //ViewBag.UsuarioSessao = UsuarioSessao;

            return View(new AgendamentosModel
            {
                Agendar = dadosAbertos,
                Usuario = UsuarioSessao,
                id_Time2 = 0,
                Quadra = Quadras.NM_Quadra,
                id_Quadra = Quadras.Id,
                id_Time1 = UsuarioSessao.Id
            });

        }

        [HttpPost]
        public IActionResult EditarJogoAberto(AgendamentosModel agendamentos)
        {
            try
            {
                var Agendado = _agendamentoRepositorio.BuscarPorId(agendamentos.Agendar.id);

                var listaAgendamentos = new List<AgendamentosModel>();

                var jaTemAgendadoTime1x1 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime1(agendamentos.Agendar.idTime1);
                listaAgendamentos.AddRange(jaTemAgendadoTime1x1);


                if (agendamentos.id_Time2 > 0)
                {
                    var jaTemAgendadoTime1x2 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime1((int)agendamentos.id_Time2);
                    listaAgendamentos.AddRange(jaTemAgendadoTime1x2);
                }

                var jaTemAgendadoTime1MenosoAtual = listaAgendamentos.Where(m => m.Id != Agendado.Id);

                var jaExisteData = false;

                foreach (var a in jaTemAgendadoTime1MenosoAtual)
                {
                    if (a.DT_Agendamento == agendamentos.Agendar.Data)
                    {
                        jaExisteData = true;
                    }
                }

                if (agendamentos.id_Time2 > 0)
                {
                    var jaTemAgendadoTime2x1 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime2(agendamentos.Agendar.idTime1);
                    var jaTemAgendadoTime2x2 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime2((int)agendamentos.id_Time2);

                    var listaAgendamentos2 = new List<AgendamentosModel>();

                    listaAgendamentos2.AddRange(jaTemAgendadoTime2x1);
                    listaAgendamentos2.AddRange(jaTemAgendadoTime2x2);

                    var jaTemAgendadoTime2MenosoAtual = listaAgendamentos2.Where(m => m.Id != Agendado.Id);

                    foreach (var a in jaTemAgendadoTime2MenosoAtual)
                    {
                        if (a.DT_Agendamento == agendamentos.Agendar.Data)
                        {
                            jaExisteData = true;

                        }
                    }

                }

                if (!jaExisteData)
                {
                    if (Agendado != null)
                    {
                        Agendado.id_Time2 = agendamentos.id_Time2 == Agendado.id_Time2 ? Agendado.id_Time2 : agendamentos.id_Time2;
                        Agendado.HR_Agendamento = agendamentos.Agendar.Hora == Agendado.HR_Agendamento ? Agendado.HR_Agendamento : agendamentos.Agendar.Hora;
                        Agendado.DT_Agendamento = agendamentos.Agendar.Data == Agendado.DT_Agendamento ? Agendado.DT_Agendamento : agendamentos.Agendar.Data;
                        Agendado.DS_Descricao = agendamentos.Agendar.DS_Descrição == Agendado.DS_Descricao ? Agendado.DS_Descricao : agendamentos.Agendar.DS_Descrição;


                        var resultado = _agendamentoRepositorio.Atualizar(Agendado);
                        TempData["MensagemSucesso"] = "Jogo editado com Sucesso";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Erro ao marcar o jogo, tente novamente";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["MensagemErro"] = "Já existe jogo marcado para essa data";
                    return RedirectToAction("Index");
                }

            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar seu Jogo, tente novamente. Detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }


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

            return View(new AgendamentosModel
            {
                UsuarioId = UsuarioSessao.Id,
                Agendar = dadosMarcados
            });

        }


    }
}



