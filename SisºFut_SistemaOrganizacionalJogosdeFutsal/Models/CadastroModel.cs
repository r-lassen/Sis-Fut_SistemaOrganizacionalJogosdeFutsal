﻿using SisºFut_SistemaOrganizacionalJogosdeFutsal.Enums;
using System.ComponentModel.DataAnnotations;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{
    public class CadastroModel
    {

        public UsuarioModel usuario { get; set; }

        public QuadrasModel quadras { get; set; }

        public PerfilEnum Perfil { get; set; }
        //public string NM_Quadra { get; set; }

        //public string DS_Endereco { get; set; }

    }
}
