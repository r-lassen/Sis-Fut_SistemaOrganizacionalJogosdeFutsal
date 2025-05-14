using Microsoft.EntityFrameworkCore;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Data;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;
using System.Linq;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public class ContatoRepositorio : IContatoRepositorio
    {
        private readonly BancoContext _bancoContext;
        public ContatoRepositorio(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }

        public ContatoModel BuscarPorId(int id)
        {
            return _bancoContext.Contatos.FirstOrDefault(x => x.Id == id);
        }

        public List<ContatoModel> BuscarTodos()
        {
            return _bancoContext.Contatos.ToList();
        }

        //public ContatoModel Adicionar(ContatoModel contato)
        //{
        //    //Gravar no banco de Dados
        //    _bancoContext.Contatos.Add(contato);
        //    _bancoContext.SaveChanges();
        //    return contato;
        //}

        public ContatoModel BuscarPorCelular(string celular)
        {
            return _bancoContext.Contatos.FirstOrDefault(c => c.Celular == celular);
        }
        public ContatoModel BuscarPorEmail(string email)
        {
            return _bancoContext.Contatos.FirstOrDefault(c => c.Email == email);
        }

        public ContatoModel Adicionar(ContatoModel contato)
        {
            // Gravando no banco de dados
            _bancoContext.Contatos.Add(contato);
            _bancoContext.SaveChanges();

            return contato;
        }

        public ContatoModel Atualizar(ContatoModel contato)
        {
            ContatoModel contatoDB = BuscarPorId(contato.Id);
            if (contatoDB == null) throw new System.Exception("Erro na Atualização do contato");

            contatoDB.Nome = contato.Nome;
            contatoDB.Email = contato.Email;
            contatoDB.Celular = contato.Celular;

            _bancoContext.Contatos.Update(contatoDB);
            _bancoContext.SaveChanges();
            return contatoDB;
        }


        public bool Apagar(int id)
        {
            ContatoModel contatoDB = BuscarPorId(id);
            if (contatoDB == null)
                throw new System.Exception("Erro em apagar contato");

            // Removendo o contato
            _bancoContext.Contatos.Remove(contatoDB);
            _bancoContext.SaveChanges();

            //// Resetando o AUTO_INCREMENT para o próximo ID disponível
            //ResetarAutoIncrement();

            return true;
        }


        //public void ResetarAutoIncrement()
        //{
        //    // Executando o comando SQL para resetar o AUTO_INCREMENT
        //    _bancoContext.Database.ExecuteSqlRaw("ALTER TABLE Contatos AUTO_INCREMENT = 1");
        //}
    }
}
