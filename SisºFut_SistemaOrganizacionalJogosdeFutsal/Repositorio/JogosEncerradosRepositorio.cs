using Microsoft.EntityFrameworkCore;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Data;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;
using System.Linq;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public class JogosEncerradosRepositorio : IJogosEncerradosRepositorio
    {
        private readonly BancoContext _bancoContext;

        public JogosEncerradosRepositorio(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }

        public JogosEncerradosModel BuscarPorId(int id)
        {
            return _bancoContext.JogosEncerrados.FirstOrDefault(x => x.Id == id);
        }


        public List<JogosEncerradosModel> BuscarTodos()
        {
            return _bancoContext.JogosEncerrados.ToList();
        }


        public List<JogosEncerradosModel> BuscarJogosAbertosPorIdTime1(int id)
        {
            return _bancoContext.JogosEncerrados.Where(x => x.IdTime1 == id).ToList();
        }

        public List<JogosEncerradosModel> BuscarJogosAbertosPorIdTime2(int id)
        {
            return _bancoContext.JogosEncerrados.Where(x => x.IdTime2 == id).ToList();
        }



        public JogosEncerradosModel Adicionar(JogosEncerradosModel JogosEncerrados)
        {
            // Gravando no banco de dados
            _bancoContext.JogosEncerrados.Add(JogosEncerrados);
            _bancoContext.SaveChanges();

            return JogosEncerrados;
        }


    }
}
