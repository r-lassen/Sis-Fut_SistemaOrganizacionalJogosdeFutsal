using System;
//Add-Migration CriacaoTabela------- -Context BancoContext
//Update-Database -ContextBanco Context

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
