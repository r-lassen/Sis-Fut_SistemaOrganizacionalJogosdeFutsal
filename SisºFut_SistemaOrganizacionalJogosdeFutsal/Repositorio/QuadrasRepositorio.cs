using Microsoft.EntityFrameworkCore;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Data;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;
using System.Linq;
namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public class QuadrasRepositorio : IQuadrasRepositorio
    {
        private readonly BancoContext _bancoContext;

        public QuadrasRepositorio(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }

        public List<QuadrasModel> BuscarTodos()
        {
            return _bancoContext.Quadras.ToList();
        }

        public QuadrasModel BuscarPorId(int id)
        {
            return _bancoContext.Quadras.FirstOrDefault(x => x.id_Time == id);
        }

        public QuadrasModel BuscarPorIdQuadra(int id)
        {
            return _bancoContext.Quadras.FirstOrDefault(x => x.Id == id);
        }


        public QuadrasModel Adicionar(QuadrasModel quadras)
        {
            // Gravando no banco de dados
            _bancoContext.Quadras.Add(quadras);
            _bancoContext.SaveChanges();

            return quadras;
        }

        public QuadrasModel Atualizar(QuadrasModel quadras)
        {
            QuadrasModel quadrasDB = BuscarPorIdQuadra(quadras.Id);
            if (quadrasDB == null) throw new System.Exception("Erro na Atualização da Quadra");

            quadrasDB.NM_Quadra = quadras.NM_Quadra;
            quadrasDB.DS_Endereco = quadras.DS_Endereco;
            quadrasDB.id_Time = quadras.id_Time;

            _bancoContext.Quadras.Update(quadrasDB);
            _bancoContext.SaveChanges();
            return quadrasDB;
        }


        public bool Apagar(int id)
        {
            QuadrasModel quadrasDB = BuscarPorId(id);
            if (quadrasDB == null)
                throw new System.Exception("Erro em apagar contato");

            // Removendo o contato
            _bancoContext.Quadras.Remove(quadrasDB);
            _bancoContext.SaveChanges();

            return true;
        }

    }
}
