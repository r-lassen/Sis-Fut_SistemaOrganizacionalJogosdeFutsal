using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Data;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Enums;
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
        private readonly IQuadrasRepositorio _quadrasRepositorio;
        private readonly IAgendamentoRepositorio _agendamentoRepositorio;
        private readonly IJogosEncerradosRepositorio _jogosEncerradosRepositorio;
        public HomeController(IUsuarioRepositorio usuarioRepositorio,
            ISessao sessao,
            IQuadrasRepositorio quadras,
            IAgendamentoRepositorio agendamentos,
            IJogosEncerradosRepositorio jogosEncerradosRepositorio)

        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
            _quadrasRepositorio = quadras;
            _agendamentoRepositorio = agendamentos;
            _jogosEncerradosRepositorio = jogosEncerradosRepositorio;
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

            var abertos = _agendamentoRepositorio.ListarAbertos()
                            .Where(a => a.st_ativo) // Só abertos ativos
                            .ToList();

            var marcados = _agendamentoRepositorio.ListarMarcados()
                            .Where(m => m.st_ativo) // Só marcados ativos
                            .ToList();

            List<DadosAgendamentos> dadosAbertos = new List<DadosAgendamentos>();
            List<DadosAgendamentos> dadosMarcados = new List<DadosAgendamentos>();

            if (abertos.Count > 0)
            {
                foreach (var aberto in abertos)
                {
                    var time1 = _usuarioRepositorio.BuscarPorId(aberto.id_Time1);
                    var quadra = _quadrasRepositorio.BuscarPorIdQuadra(aberto.id_Quadra);

                    // Só adiciona se time1 estiver ativo
                    if (time1 != null && time1.st_ativo)
                    {
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
                }
            }

            if (marcados.Count > 0)
            {
                foreach (var marcado in marcados)
                {
                    var time1 = _usuarioRepositorio.BuscarPorId(marcado.id_Time1);
                    var time2 = _usuarioRepositorio.BuscarPorId(marcado.id_Time2.Value);
                    var quadra = _quadrasRepositorio.BuscarPorIdQuadra(marcado.id_Quadra);

                    // Só adiciona se time1 e time2 estiverem ativos
                    if (time1 != null && time1.st_ativo && time2 != null && time2.st_ativo)
                    {
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
                }
            }

            HomeModel home = new HomeModel
            {
                Nome = usuarioLogado.Name,
                Email = usuarioLogado.Email,
                Id = usuarioLogado.Id,
                Abertos = dadosAbertos,
                Marcados = dadosMarcados,
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


            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Login", "Conta"); // Redireciona para login se não estiver logado
            }
            var usuarioSessao = _sessao.BuscarSessaoDoUsuario();

            var quadra = _quadrasRepositorio.BuscarPorId(usuarioSessao.Id);

            // Verifica se a quadra existe e se possui dados obrigatórios
            if (quadra != null &&
                !string.IsNullOrWhiteSpace(quadra.NM_Quadra) &&
                !string.IsNullOrWhiteSpace(quadra.DS_Endereco))
            {
                var usuariosAll = _usuarioRepositorio.BuscarTodos();

                // Filtra para não incluir o próprio usuário, nem Admins, e somente usuários ativos
                var usuariosFiltrados = usuariosAll
                    .Where(m => m.Id != usuarioSessao.Id && m.Perfil != PerfilEnum.Admin && m.st_ativo)
                    .ToList();

                ViewBag.UserList = new SelectList(usuariosFiltrados, "Id", "Name");

                return View(new AgendamentosModel
                {
                    Usuario = usuarioSessao,
                    id_Time1 = usuarioSessao.Id,
                    id_Time2 = 0,
                    id_Quadra = quadra.Id,
                    Quadra = quadra.NM_Quadra
                });
            }
            else
            {
                TempData["MensagemErro"] = "Para agendar um Amistoso, cadastre uma Quadra.";
                return RedirectToAction("Index");
            }
        }



        [HttpPost]
        public IActionResult Amistoso(AgendamentosModel agendamentos)
        {
            try
            {

                var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

                if (usuarioLogado == null)
                {
                    return RedirectToAction("Login", "Conta"); // Redireciona para login se não estiver logado
                }

                // Carrega a lista de usuários excluindo Admins e desativados, uma única vez
                var usuarios = _usuarioRepositorio.BuscarTodos();
                var usuariosFiltrados = usuarios.Where(u => u.Perfil != PerfilEnum.Admin && u.st_ativo == true).ToList();
                ViewBag.UserList = new SelectList(usuariosFiltrados, "Id", "Name");

                // Verifica se os times estão ativos
                var time1 = _usuarioRepositorio.BuscarPorId(agendamentos.id_Time1);
                if (time1 == null || !time1.st_ativo)
                {
                    TempData["MensagemErro"] = "O time 1 está desativado ou inválido.";
                    return View(agendamentos);
                }

                if (agendamentos.id_Time2.HasValue && agendamentos.id_Time2 > 0)
                {
                    var time2 = _usuarioRepositorio.BuscarPorId((int)agendamentos.id_Time2);
                    if (time2 == null || !time2.st_ativo)
                    {
                        TempData["MensagemErro"] = "O time 2 está desativado ou inválido.";
                        return View(agendamentos);
                    }
                }

                if (agendamentos.DT_Agendamento < DateTime.Now)
                {
                    TempData["MensagemErro"] = "Selecione uma data válida.";
                    return View(agendamentos);
                }

                // Verifica se a hora está vazia/nula ou no formato inválido
                if (string.IsNullOrEmpty(agendamentos.HR_Agendamento) || !agendamentos.HR_Agendamento.Contains(":"))
                {
                    TempData["MensagemErro"] = "Selecione uma hora válida (formato HH:MM).";
                    return View(agendamentos);
                }

                // Divide a hora e valida os valores
                var partesHora = agendamentos.HR_Agendamento.Split(':');
                if (partesHora.Length != 2 ||
                    !int.TryParse(partesHora[0], out int horas) ||
                    !int.TryParse(partesHora[1], out int minutos) ||
                    horas < 0 || horas > 23 ||
                    minutos < 0 || minutos > 59)
                {
                    TempData["MensagemErro"] = "Hora inválida. Use o formato HH:MM (ex: 14:30).";
                    return View(agendamentos);
                }

                // Verifica se já existe jogo marcado para a data (para os dois times)
                var jaTemAgendadoTime1 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime1(agendamentos.id_Time1);
                var jaExisteData = jaTemAgendadoTime1.Any(a => a.DT_Agendamento.Date == agendamentos.DT_Agendamento.Date);

                if (agendamentos.id_Time2 > 0)
                {
                    var jaTemAgendadoTime2 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime2((int)agendamentos.id_Time2);
                    if (jaTemAgendadoTime2.Any(a => a.DT_Agendamento.Date == agendamentos.DT_Agendamento.Date))
                    {
                        jaExisteData = true;
                    }
                }

                if (!jaExisteData)
                {
                    // Setar agendamento como ativo
                    agendamentos.st_ativo = true;

                    var resultado = _agendamentoRepositorio.Adicionar(agendamentos);
                    TempData["MensagemSucesso"] = "Jogo cadastrado com sucesso!";
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
                TempData["MensagemErro"] = $"Erro ao cadastrar seu jogo, tente novamente. Detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }



        //---------------Essa carrega a view--------------
        public IActionResult JogoAberto(int id)
        {

            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Login", "Conta"); // Redireciona para login se não estiver logado
            }
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

            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Login", "Conta"); // Redireciona para login se não estiver logado
            }
            var Agendamento = _agendamentoRepositorio.BuscarPorId(id);

            var time1 = _usuarioRepositorio.BuscarPorId(Agendamento.id_Time1);
            var quadra = _quadrasRepositorio.BuscarPorIdQuadra(Agendamento.id_Quadra);

            DadosAgendamentos dadosAbertos = new DadosAgendamentos()
            {
                Time1 = time1.Name,
                Local = quadra.NM_Quadra + "-" + quadra.DS_Endereco,
                Data = Agendamento.DT_Agendamento,
                DS_Descrição = Agendamento.DS_Descricao,
                Hora = Agendamento.HR_Agendamento,
                id = Agendamento.Id,
                FotoTime1 = time1.Foto,
                idTime1 = time1.Id
            };

            var UsuarioSessao = _sessao.BuscarSessaoDoUsuario();

            var usuariosAll = _usuarioRepositorio.BuscarTodos();

            // Filtra para excluir o próprio usuário, perfis Admin e incluir só ativos
            var usuariosFiltrados = usuariosAll
                .Where(m => m.Id != UsuarioSessao.Id && m.Perfil != PerfilEnum.Admin && m.st_ativo)
                .ToList();

            ViewBag.UserList = new SelectList(usuariosFiltrados, "Id", "Name");

            var Quadras = _quadrasRepositorio.BuscarPorId(UsuarioSessao.Id);

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
                // Validação da Hora
                if (string.IsNullOrEmpty(agendamentos.Agendar.Hora) || !agendamentos.Agendar.Hora.Contains(":"))
                {
                    TempData["MensagemErro"] = "Selecione uma hora válida (formato HH:MM).";
                    return RedirectToAction("EditarJogoAberto");
                }

                var partesHora = agendamentos.Agendar.Hora.Split(':');
                if (partesHora.Length != 2 ||
                    !int.TryParse(partesHora[0], out int horas) ||
                    !int.TryParse(partesHora[1], out int minutos) ||
                    horas < 0 || horas > 23 ||
                    minutos < 0 || minutos > 59)
                {
                    TempData["MensagemErro"] = "Hora inválida. Use o formato HH:MM (ex: 14:30).";
                    return RedirectToAction("EditarJogoAberto");
                }

                // Validação da Data
                if (agendamentos.Agendar.Data == default || agendamentos.Agendar.Data < DateTime.Today)
                {
                    TempData["MensagemErro"] = "Selecione uma data válida (não pode ser anterior ao dia atual).";
                    return RedirectToAction("EditarJogoAberto");
                }

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
                    return RedirectToAction("EditarJogoAberto");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar seu Jogo, tente novamente. Detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        //---------------Essa carrega a view--------------
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

        [HttpPost]
        public IActionResult JogoMarcado(AgendamentosModel agendamentos)
        {
            try
            {
                // Validação da Hora
                if (string.IsNullOrEmpty(agendamentos.Agendar.Hora) || !agendamentos.Agendar.Hora.Contains(":"))
                {
                    TempData["MensagemErro"] = "Selecione uma hora válida (formato HH:MM).";
                    return RedirectToAction("JogoMarcado");
                }

                var partesHora = agendamentos.Agendar.Hora.Split(':');
                if (partesHora.Length != 2 ||
                    !int.TryParse(partesHora[0], out int horas) ||
                    !int.TryParse(partesHora[1], out int minutos) ||
                    horas < 0 || horas > 23 ||
                    minutos < 0 || minutos > 59)
                {
                    TempData["MensagemErro"] = "Hora inválida. Use o formato HH:MM (ex: 14:30).";
                    return RedirectToAction("JogoMarcado");
                }

                // Validação da Data
                if (agendamentos.Agendar.Data == default || agendamentos.Agendar.Data < DateTime.Today)
                {
                    TempData["MensagemErro"] = "Selecione uma data válida (não pode ser anterior ao dia atual).";
                    return RedirectToAction("JogoMarcado");
                }

                var Agendado = _agendamentoRepositorio.BuscarPorId(agendamentos.Agendar.id);

                var listaAgendamentos = new List<AgendamentosModel>();

                var jaTemAgendadoTime1x1 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime1(agendamentos.Agendar.idTime1);
                listaAgendamentos.AddRange(jaTemAgendadoTime1x1);

                if (agendamentos.Agendar.idTime2 > 0)
                {
                    var jaTemAgendadoTime1x2 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime1(agendamentos.Agendar.idTime2);
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

                if (agendamentos.Agendar.idTime2 > 0)
                {
                    var jaTemAgendadoTime2x1 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime2(agendamentos.Agendar.idTime1);
                    var jaTemAgendadoTime2x2 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime2(agendamentos.Agendar.idTime2);

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
                    return RedirectToAction("JogoMarcado");
                }
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar seu Jogo, tente novamente. Detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }




        [HttpPost]
        public IActionResult EncerrarJogo(int id, int golsTime1, int golsTime2, string descricao)
        {
            try
            {

                var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

                if (usuarioLogado == null)
                {
                    return RedirectToAction("Login", "Conta"); // Redireciona para login se não estiver logado
                }
                var agendamento = _agendamentoRepositorio.BuscarPorId(id);
                if (agendamento == null)
                {
                    TempData["MensagemErro"] = "Jogo não encontrado para encerrar.";
                    return RedirectToAction("Index");
                }

                // Validação dos gols (não negativos)
                if (golsTime1 < 0 || golsTime2 < 0)
                {
                    TempData["MensagemErro"] = "Os gols não podem ser negativos.";
                    return RedirectToAction("Index");
                }

                var jogoEncerrado = new JogosEncerradosModel
                {
                    IdTime1 = agendamento.id_Time1,
                    IdTime2 = agendamento.id_Time2 ?? 0,
                    GolsTime1 = golsTime1,
                    GolsTime2 = golsTime2,
                    IdQuadra = agendamento.id_Quadra,
                    Descricao = descricao
                };

                var resultado = _jogosEncerradosRepositorio.Adicionar(jogoEncerrado);
                if (resultado != null)
                {
                    // Atualiza o st_ativo para false e atualiza a data
                    agendamento.st_ativo = false;
                    agendamento.DT_Atualizacao = DateTime.Now;

                    _agendamentoRepositorio.Atualizar(agendamento);

                    TempData["MensagemSucesso"] = "Jogo encerrado com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao encerrar o jogo. Tente novamente.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao encerrar o jogo. Detalhes: {ex.Message}";
                return RedirectToAction("Index");
            }
        }



        [HttpPost]
        public IActionResult ExcluirJogo(int id)
        {
            try
            {


                var usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                if (usuarioLogado == null)
                {
                    TempData["MensagemErro"] = "Usuário não encontrado na sessão.";
                    return RedirectToAction("Index", "Login");
                }

                var jogo = _agendamentoRepositorio.BuscarPorId(id); // Certifique-se de que tem esse método no repositório
                if (jogo == null)
                {
                    TempData["MensagemErro"] = "Jogo não encontrado.";
                    return RedirectToAction("Index", "Home");
                }

                if (jogo.id_Time1 != usuarioLogado.Id)
                {
                    TempData["MensagemErro"] = "Você não tem permissão para excluir este jogo.";
                    return RedirectToAction("Index", "Home");
                }

                bool apagado = _agendamentoRepositorio.Apagar(id);
                if (apagado)
                {
                    TempData["MensagemSucesso"] = "Jogo excluído com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Não foi possível excluir o jogo.";
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao excluir o jogo: {erro.Message}";
                return RedirectToAction("Index", "Home");
            }
        }











    }
}



