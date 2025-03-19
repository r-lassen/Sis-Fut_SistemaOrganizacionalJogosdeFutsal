using Microsoft.EntityFrameworkCore;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Data;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;
using System.Linq;


namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public class TimeXQuadrasRepositorio : ITimeXQuadrasRepositorio
    {
        private readonly BancoContext _bancoContext;

        public TimeXQuadrasRepositorio(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }



        public List<TimeXQuadrasModel> BuscarTodos()
        {
            return _bancoContext.TimexQuadras.ToList();
        }

        public TimeXQuadrasModel BuscarPorId(int id)
        {
            return _bancoContext.TimexQuadras.FirstOrDefault(x => x.Id == id);
        }

        public TimeXQuadrasModel BuscarPorTime(int id)
        {
            return _bancoContext.TimexQuadras.FirstOrDefault(x => x.id_Time == id);
        }

        public TimeXQuadrasModel Adicionar(TimeXQuadrasModel timesXquadras)
        {
            // Gravando no banco de dados
            _bancoContext.TimexQuadras.Add(timesXquadras);
            _bancoContext.SaveChanges();

            return timesXquadras;
        }

        public TimeXQuadrasModel Atualizar(TimeXQuadrasModel timexquadras)
        {
            TimeXQuadrasModel timexquadrasdb = BuscarPorId(timexquadras.Id);
            if (timexquadrasdb == null) throw new System.Exception("erro na atualização do contato");

            timexquadrasdb.id_Time = timexquadras.id_Time;
            timexquadrasdb.id_Quadra = timexquadras.id_Quadra;


            _bancoContext.TimexQuadras.Update(timexquadrasdb);
            _bancoContext.SaveChanges();
            return timexquadrasdb;
        }


        public bool Apagar(int id)
        {
            TimeXQuadrasModel TimexQuadrasDB = BuscarPorId(id);
            if (TimexQuadrasDB == null)
                throw new System.Exception("Erro em apagar contato");

            // Removendo o contato
            _bancoContext.TimexQuadras.Remove(TimexQuadrasDB);
            _bancoContext.SaveChanges();

            return true;
        }    















    }
}
