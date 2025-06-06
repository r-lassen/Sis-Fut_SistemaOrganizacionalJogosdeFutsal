using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Data;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Filters;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using X.PagedList;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    [PaginaRestritaAdm]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IQuadrasRepositorio _quadrasRepositorio;
        private readonly IAgendamentoRepositorio _agendamentoRepositorio;


        public UsuarioController(IUsuarioRepositorio usuarioRepositorio,
             IQuadrasRepositorio quadras,
             IAgendamentoRepositorio agendamentoRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _quadrasRepositorio = quadras;
            _agendamentoRepositorio = agendamentoRepositorio;
        }

        public IActionResult Index(string filtro, int? page)
        {

            int pageNumber = page ?? 1;
            int pageSize = 10;

            var usuarios = _usuarioRepositorio.BuscarTodos().AsQueryable();

            if (!string.IsNullOrEmpty(filtro))
            {
                usuarios = usuarios
                    .Where(u => u.Name.Contains(filtro, StringComparison.OrdinalIgnoreCase));
            }

            var usuariosPaginados = usuarios
                .OrderBy(u => u.Name)
                .ToPagedList(pageNumber, pageSize);

            return View(usuariosPaginados);
        }

        public IActionResult Filtrar(string filtro, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var usuarios = _usuarioRepositorio.BuscarTodos()
                                             .Where(u => string.IsNullOrEmpty(filtro) || u.Name.Contains(filtro, StringComparison.OrdinalIgnoreCase))
                                             .OrderBy(u => u.Name)
                                             .ToPagedList(pageNumber, pageSize);

            return PartialView("_TabelaUsuarios", usuarios);
        }


        public IActionResult Criar()
        {
            return View();
        }

        public IActionResult Editar(int id)
        {
            var usuario = _usuarioRepositorio.BuscarPorId(id);

            if (usuario == null)
            {
                TempData["MensagemErro"] = "Usuário não encontrado.";
                return RedirectToAction("Index");
            }

            // Mapeia os dados para a model sem senha
            var usuarioSemSenha = new UsuarioSemSenhaModel
            {
                Id = usuario.Id,
                Name = usuario.Name,
                Login = usuario.Login,
                Email = usuario.Email,
                Perfil = usuario.Perfil
            };

            return View(usuarioSemSenha);
        }

        public IActionResult ApagarConfirmacao(int id)
        {
            UsuarioModel usuario = _usuarioRepositorio.BuscarPorId(id);
            return View(usuario);
        }

        public IActionResult Apagar(int id)
        {
            try
            {
                // Busca o usuário pelo ID (que o ADM quer excluir)
                var usuario = _usuarioRepositorio.BuscarPorId(id);
                if (usuario == null)
                {
                    TempData["MensagemErro"] = "Usuário não encontrado.";
                    return RedirectToAction("Index");
                }

                // Verifica se o usuário tem quadra associada
                var quadraUsuario = _quadrasRepositorio.BuscarPorId(id);
                if (quadraUsuario != null)
                {
                    var quadraExcluida = _quadrasRepositorio.Apagar(quadraUsuario.Id);
                    if (!quadraExcluida)
                    {
                        TempData["MensagemErro"] = "Não foi possível excluir a quadra do usuário.";
                        return RedirectToAction("Index");
                    }
                }

                // Verifica agendamentos vinculados ao usuário (como time1 ou time2)
                var agendamentosTime1 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime1(id);
                var agendamentosTime2 = _agendamentoRepositorio.BuscarJogosAbertosPorIdTime2(id);
                var todosAgendamentos = agendamentosTime1.Concat(agendamentosTime2).ToList();

                if (todosAgendamentos.Count > 0)
                {
                    foreach (var agendamento in todosAgendamentos)
                    {
                        var agendamentoExcluido = _agendamentoRepositorio.Apagar(agendamento.Id);
                        if (!agendamentoExcluido)
                        {
                            TempData["MensagemErro"] = "Não foi possível excluir um agendamento vinculado.";
                            return RedirectToAction("Index");
                        }
                    }
                }

                // Exclui o usuário
                var usuarioExcluido = _usuarioRepositorio.Apagar(id);
                if (!usuarioExcluido)
                {
                    TempData["MensagemErro"] = "Não foi possível excluir o usuário.";
                    return RedirectToAction("Index");
                }

                // Sucesso!
                TempData["MensagemSucesso"] = "Usuário e todos os dados vinculados foram excluídos com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao excluir usuário: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Criar(UsuarioModel usuario)
        {
            try
            {
                // Remove espaços em branco dos campos de texto
                usuario.Name = usuario.Name?.Trim();
                usuario.Email = usuario.Email?.Trim();
                usuario.Login = usuario.Login?.Trim();
                usuario.Senha = usuario.Senha?.Trim();

                if (ModelState.IsValid)
                {
                    // Verifica se o e-mail já está cadastrado
                    var emailExiste = _usuarioRepositorio.BuscarPorEmail(usuario.Email);
                    if (emailExiste != null)
                    {
                        ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
                    }

                    // Verifica se o login já está cadastrado
                    var loginExiste = _usuarioRepositorio.BuscarPorLogin(usuario.Login);
                    if (loginExiste != null)
                    {
                        ModelState.AddModelError("Login", "Este login já está em uso.");
                    }

                    // Verifica se o nome do time já está cadastrado
                    var nomeTimeExiste = _usuarioRepositorio.BuscarPorNomeTime(usuario.Name);
                    if (nomeTimeExiste != null)
                    {
                        ModelState.AddModelError("Name", "Este nome de time já está em uso.");
                    }

                    // Se houver erros, retorna para a View com mensagens
                    if (!ModelState.IsValid)
                    {
                        return View(usuario);
                    }

                    // Adiciona o usuário
                    usuario = _usuarioRepositorio.Adicionar(usuario);

                    // Cria a quadra associada ao novo time
                    QuadrasModel quadra = new QuadrasModel
                    {
                        NM_Quadra = string.Empty,
                        DS_Endereco = string.Empty,
                        id_Time = usuario.Id
                    };

                    _quadrasRepositorio.Adicionar(quadra);

                    TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }

                return View(usuario);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar o usuário. Detalhes: {erro.Message}";
                return RedirectToAction("Index");
            }
        }



        [HttpPost]
        public IActionResult Editar(UsuarioSemSenhaModel usuario)
        {
            try
            {
                // Limpa espaços em branco dos campos antes de validar
                usuario.Name = usuario.Name?.Trim();
                usuario.Email = usuario.Email?.Trim();
                usuario.Login = usuario.Login?.Trim();

                // Valida se o ModelState está correto
                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

                var usuarioNoBanco = _usuarioRepositorio.BuscarPorId(usuario.Id);

                if (usuarioNoBanco == null)
                {
                    TempData["MensagemErro"] = "Usuário não encontrado.";
                    return RedirectToAction("Index");
                }

                // Verifica se o e-mail já está em uso por outro usuário
                var emailExiste = _usuarioRepositorio.BuscarPorEmail(usuario.Email);
                if (emailExiste != null && emailExiste.Id != usuario.Id)
                {
                    ModelState.AddModelError("Email", "Este e-mail já está cadastrado por outro usuário.");
                }

                // Verifica se o login já está em uso por outro usuário
                var loginExiste = _usuarioRepositorio.BuscarPorLogin(usuario.Login);
                if (loginExiste != null && loginExiste.Id != usuario.Id)
                {
                    ModelState.AddModelError("Login", "Este login já está em uso por outro usuário.");
                }

                // Verifica se o nome do time já está em uso por outro usuário
                var nomeTimeExiste = _usuarioRepositorio.BuscarPorNomeTime(usuario.Name);
                if (nomeTimeExiste != null && nomeTimeExiste.Id != usuario.Id)
                {
                    ModelState.AddModelError("Name", "Este nome de time já está em uso por outro usuário.");
                }

                // Se houver erro de validação, retorna à view
                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

                // Atualiza os dados
                usuarioNoBanco.Name = usuario.Name;
                usuarioNoBanco.Login = usuario.Login;
                usuarioNoBanco.Email = usuario.Email;
                usuarioNoBanco.Perfil = usuario.Perfil;

                _usuarioRepositorio.Atualizar(usuarioNoBanco);

                TempData["MensagemSucesso"] = "Usuário alterado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, erro ao atualizar o usuário. Detalhe: {erro.Message}";
                return RedirectToAction("Index");
            }
        }







    }
}
