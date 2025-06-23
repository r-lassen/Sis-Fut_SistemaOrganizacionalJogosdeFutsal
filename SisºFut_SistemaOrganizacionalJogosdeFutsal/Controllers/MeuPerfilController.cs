using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    public class MeuPerfilController : Controller
    {


        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISessao _sessao;
        private readonly IAgendamentoRepositorio _agendamentoRepositorio;
        private readonly IQuadrasRepositorio _quadrasRepositorio;


        public MeuPerfilController(
            IUsuarioRepositorio usuarioRepositorio,
            ISessao sessao,
            IQuadrasRepositorio quadras,
            IAgendamentoRepositorio agendamentos)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
            _quadrasRepositorio = quadras;
            _agendamentoRepositorio = agendamentos;
        }

        public IActionResult Index()
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Login", "Index");
            }

            var time1 = _usuarioRepositorio.BuscarPorId(usuarioLogado.Id);

            var listaAgendamentos = new List<AgendamentosModel>();

            var jaTemAgendadoTime1 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime1(usuarioLogado.Id);
            var jaTemAgendadoTime2 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime2(usuarioLogado.Id);

            if (jaTemAgendadoTime1.Count > 0)
            {
                foreach (var a in jaTemAgendadoTime1)
                {
                    if (a.st_ativo && a.id_Time2 != null)
                    {
                        listaAgendamentos.Add(a);
                    }
                }
            }

            if (jaTemAgendadoTime2.Count > 0)
            {
                foreach (var a in jaTemAgendadoTime2)
                {
                    if (a.st_ativo)
                    {
                        listaAgendamentos.Add(a);
                    }
                }
            }

            List<DadosAgendamentos> dadosMarcados = new List<DadosAgendamentos>();

            foreach (var marcado in listaAgendamentos)
            {
                var time1OK = _usuarioRepositorio.BuscarPorId(marcado.id_Time1);
                var time2OK = _usuarioRepositorio.BuscarPorId(marcado.id_Time2.Value);
                var quadra = _quadrasRepositorio.BuscarPorIdQuadra(marcado.id_Quadra);

                dadosMarcados.Add(new DadosAgendamentos
                {
                    Time1 = time1OK?.Name,
                    Time2 = time2OK?.Name,
                    Local = quadra != null ? quadra.NM_Quadra + "-" + quadra.DS_Endereco : "",
                    Data = marcado.DT_Agendamento,
                    DS_Descrição = marcado.DS_Descricao,
                    Hora = marcado.HR_Agendamento,
                    id = marcado.Id,
                    FotoTime1 = time1OK?.Foto,
                    FotoTime2 = time2OK?.Foto,
                    idTime1 = time1OK?.Id ?? 0,
                    idTime2 = time2OK?.Id ?? 0,
                });
            }

            var quadraOk = _quadrasRepositorio.BuscarPorId(usuarioLogado.Id);

            HomeModel home = new HomeModel
            {
                Quadra = quadraOk,
                Usuario = time1,
                Marcados = dadosMarcados,
            };

            return View(home);
        }


        public IActionResult Editar()
        {

            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Login", "Conta"); // Redireciona para login se não estiver logado
            }

            var quadraOk = _quadrasRepositorio.BuscarPorId(usuarioLogado.Id);


            HomeModel home = new HomeModel
            {

                Quadra = quadraOk,
                Usuario = usuarioLogado,

            };



            return View(home);
        }

        [HttpPost]
        public IActionResult Editar(HomeModel homemodel)
        {
            try
            {
                var usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                var quadraExistente = _quadrasRepositorio.BuscarPorId(usuarioLogado.Id);

                // Verificações de duplicidade
                var emailExiste = _usuarioRepositorio.BuscarPorEmail(homemodel.Usuario.Email);
                if (emailExiste != null && emailExiste.Id != homemodel.Usuario.Id)
                {
                    ModelState.AddModelError("Usuario.Email", "Este e-mail já está cadastrado por outro usuário.");
                }

                // Sugestão para domínio de e-mail inválido
                var sugestao = EmailHelper.SugerirDominioCorreto(homemodel.Usuario.Email);
                if (sugestao != null)
                {
                    ModelState.AddModelError("Usuario.Email", $"Domínio inválido. Você quis dizer: {sugestao}?");
                }

                var loginExiste = _usuarioRepositorio.BuscarPorLogin(homemodel.Usuario.Login);
                if (loginExiste != null && loginExiste.Id != homemodel.Usuario.Id)
                {
                    ModelState.AddModelError("Usuario.Login", "Este login já está em uso por outro usuário.");
                }

                var nomeTimeExiste = _usuarioRepositorio.BuscarPorNomeTime(homemodel.Usuario.Name);
                if (nomeTimeExiste != null && nomeTimeExiste.Id != homemodel.Usuario.Id)
                {
                    ModelState.AddModelError("Usuario.Name", "Este nome de time já está em uso por outro usuário.");
                }

                if (!ModelState.IsValid)
                {
                    return View(homemodel);
                }

                // Conversão da imagem para Base64
                var base64 = string.Empty;
                if (homemodel.Usuario.FotoArquivo != null && homemodel.Usuario.FotoArquivo.Length > 0)
                {
                    base64 = ConverterParaBase64(homemodel.Usuario.FotoArquivo);
                }

                // Atualiza os dados do usuário
                var usuario = new UsuarioModel
                {
                    Id = homemodel.Usuario.Id,
                    Name = homemodel.Usuario.Name,
                    Login = homemodel.Usuario.Login,
                    Email = homemodel.Usuario.Email,
                    Perfil = homemodel.Usuario.Perfil,
                    DataCadastro = homemodel.Usuario.DataCadastro,
                    DataAtualização = DateTime.Now,
                    Senha = homemodel.Usuario.Senha,
                    Foto = string.IsNullOrEmpty(base64)
                        ? _usuarioRepositorio.BuscarPorId(usuarioLogado.Id).Foto
                        : base64
                };

                // Atualiza os dados da quadra, mesmo se estiverem em branco (serão salvos como null)
                if (quadraExistente != null)
                {
                    quadraExistente.NM_Quadra = string.IsNullOrWhiteSpace(homemodel.Quadra?.NM_Quadra) ? null : homemodel.Quadra.NM_Quadra.Trim();
                    quadraExistente.DS_Endereco = string.IsNullOrWhiteSpace(homemodel.Quadra?.DS_Endereco) ? null : homemodel.Quadra.DS_Endereco.Trim();
                    _quadrasRepositorio.Atualizar(quadraExistente);
                }

                _usuarioRepositorio.Atualizar(usuario);
                _sessao.CriarSessaoDoUsuario(usuario);

                TempData["MensagemSucesso"] = "Usuário alterado com sucesso!";
                return RedirectToAction("Index", "MeuPerfil");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos atualizar seu usuário, tente novamente. Detalhe: {erro.Message}";
                return RedirectToAction("Index", "MeuPerfil");
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

        public IActionResult JogoMarcado(int id)
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Login", "Conta"); // Redireciona para login se não estiver logado
            }

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
                    return RedirectToAction("Index");
                }

            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar seu Jogo, tente novamente. Detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }


        }

        public IActionResult _ModalExcluirPerfil(int id)
        {
            UsuarioModel usuario = _usuarioRepositorio.BuscarPorId(id);
            return View(usuario);
        }

        public IActionResult ExcluirPerfil()
        {
            try
            {
                var usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                if (usuarioLogado == null)
                {
                    TempData["MensagemErro"] = "Usuário não encontrado na sessão.";
                    return RedirectToAction("Index", "Login");
                }

                var quadraOk = _quadrasRepositorio.BuscarPorId(usuarioLogado.Id);
                var usuariook = _usuarioRepositorio.BuscarPorId(usuarioLogado.Id);

                if (usuariook == null)
                {
                    TempData["MensagemErro"] = "Usuário não encontrado no banco de dados.";
                    return RedirectToAction("Index", "Perfil");
                }

                var listaAgendamentos = new List<AgendamentosModel>();
                var jaTemAgendadoTime1 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime1(usuarioLogado.Id);
                var jaTemAgendadoTime2 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime2(usuarioLogado.Id);

                if (jaTemAgendadoTime1.Count > 0) listaAgendamentos.AddRange(jaTemAgendadoTime1);
                if (jaTemAgendadoTime2.Count > 0) listaAgendamentos.AddRange(jaTemAgendadoTime2);

                // Atualiza st_ativo da quadra para false (se existir)
                if (quadraOk != null)
                {
                    quadraOk.st_ativo = false;
                    var quadraAtualizada = _quadrasRepositorio.Atualizar(quadraOk);
                    if (quadraAtualizada == null)
                    {
                        TempData["MensagemErro"] = "Não foi possível desativar a quadra associada.";
                        return RedirectToAction("Index", "Perfil");
                    }
                }

                // Atualiza st_ativo de todos os agendamentos para false
                foreach (var agendamento in listaAgendamentos)
                {
                    agendamento.st_ativo = false;
                    agendamento.DT_Atualizacao = DateTime.Now; // opcional, se tiver campo de atualização
                    var agendamentoAtualizado = _agendamentoRepositorio.Atualizar(agendamento);
                    if (agendamentoAtualizado == null)
                    {
                        TempData["MensagemErro"] = "Não foi possível desativar um agendamento pendente.";
                        return RedirectToAction("Index", "Perfil");
                    }
                }

                // Atualiza st_ativo do usuário para false
                usuariook.st_ativo = false;
                var usuarioAtualizado = _usuarioRepositorio.Atualizar(usuariook);
                if (usuarioAtualizado == null)
                {
                    TempData["MensagemErro"] = "Não foi possível desativar o perfil.";
                    return RedirectToAction("Index", "Perfil");
                }

                // Remove sessão do usuário
                _sessao.RemoverSessaoUsuario();
                TempData["MensagemSucesso"] = "Perfil desativado com sucesso!";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao desativar perfil: {ex.Message}");
                TempData["MensagemErro"] = "Ocorreu um erro ao tentar desativar o perfil. Tente novamente mais tarde.";
                return RedirectToAction("Index", "Perfil");
            }
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
