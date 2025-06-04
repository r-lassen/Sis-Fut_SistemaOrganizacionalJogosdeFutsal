﻿using Microsoft.EntityFrameworkCore;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Data;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public class AgendamentoRepositorio : IAgendamentoRepositorio
    {
        private readonly BancoContext _bancoContext;

        public AgendamentoRepositorio(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }

        public AgendamentosModel BuscarPorId(int id)
        {
            return _bancoContext.Agendamentos.FirstOrDefault(x => x.Id == id);
        }


        public List<AgendamentosModel> BuscarTodos()
        {
            return _bancoContext.Agendamentos.ToList();
        }

        public List<AgendamentosModel> BuscarJogosAbertosPorIdTime1(int id)
        {
            return _bancoContext.Agendamentos.Where(x => x.id_Time1 == id).ToList();
        }

        public List<AgendamentosModel> BuscarJogosAbertosPorIdTime2(int id)
        {
            return _bancoContext.Agendamentos.Where(x => x.id_Time2 == id).ToList();
        }

        public List<AgendamentosModel> ListarAbertos()
        {
            return _bancoContext.Agendamentos.Where(x => x.id_Time2 == null).ToList();
        }

        public List<AgendamentosModel> ListarMarcados()
        {
            return _bancoContext.Agendamentos.Where(x => x.id_Time2 != null).ToList();
        }


        public AgendamentosModel Adicionar(AgendamentosModel agendamentos)
        {
            // Gravando no banco de dados
            _bancoContext.Agendamentos.Add(agendamentos);
            _bancoContext.SaveChanges();

            return agendamentos;
        }

        public AgendamentosModel Atualizar(AgendamentosModel agendamentos)
        {
            AgendamentosModel AgendamentoDB = BuscarPorId(agendamentos.Id);
            if (AgendamentoDB == null) throw new System.Exception("Erro na Atualização do contato");

            AgendamentoDB.DS_Descricao = agendamentos.DS_Descricao;
            AgendamentoDB.DT_Atualizacao = agendamentos.DT_Atualizacao;

            _bancoContext.Agendamentos.Update(AgendamentoDB);
            _bancoContext.SaveChanges();
            return AgendamentoDB;
        }


        public bool Apagar(int id)
        {
            var agendamentoEntity = _bancoContext.Agendamentos.Find(id);
            if (agendamentoEntity == null)
                throw new Exception("Agendamento não encontrado para apagar");

            _bancoContext.Agendamentos.Remove(agendamentoEntity);
            _bancoContext.SaveChanges();

            return true;
        }

    }
}
