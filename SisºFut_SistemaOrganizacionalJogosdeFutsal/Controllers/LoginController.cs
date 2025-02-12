using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();  
        }
    }
                   
                    TempData["MensagemError"] = $"Usuário e/ou senha inválido(s). Tente Novamente.";
}

                return View("Index");
            }
            catch(Exception erro)
            {
                TempData["MensagemErro"] = $"Não conseguimos realizar seu login, Tente novamente. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
