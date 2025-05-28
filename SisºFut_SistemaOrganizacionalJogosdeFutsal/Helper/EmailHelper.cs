using DnsClient;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper
{
    public class EmailHelper
    {
        public static bool Enviar(string emailDestino, string assunto, string mensagem)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("sistemadefutsal@gmail.com", "vpmtvbemoesrhnih"), // ⚠️ Veja observação abaixo
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("sistemadefutsal@gmail.com"),
                    Subject = assunto,
                    Body = mensagem,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(emailDestino);

                smtpClient.Send(mailMessage);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> VerificarDominioEmailAsync(string email)
        {
            var dominio = email.Split('@').LastOrDefault();
            if (string.IsNullOrWhiteSpace(dominio)) return false;

            var lookup = new LookupClient();
            var resultado = await lookup.QueryAsync(dominio, QueryType.MX);
            return resultado.Answers.MxRecords().Any();
        }

        // Lista de domínios válidos e comuns
        private static readonly List<string> DominiosValidos = new List<string>
    {
        "gmail.com", "hotmail.com", "outlook.com", "live.com", "yahoo.com",
        "icloud.com", "uol.com.br", "bol.com.br", "terra.com.br", "globo.com",
        "r7.com", "protonmail.com", "zoho.com", "fastmail.com"
    };

        // Verifica e sugere correção para domínios
        public static string SugerirDominioCorreto(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@")) return null;

            var partes = email.Split('@');
            if (partes.Length != 2) return null;

            var nome = partes[0];
            var dominio = partes[1];

            // Verifica se é um domínio válido
            if (DominiosValidos.Contains(dominio.ToLower()))
            {
                return null; // Está ok
            }

            // Tenta encontrar o domínio mais parecido
            var sugestao = DominiosValidos
                .OrderBy(d => ObterDistanciaLevenshtein(dominio.ToLower(), d))
                .FirstOrDefault();

            return $"{nome}@{sugestao}";
        }

        // Distância de Levenshtein simples
        private static int ObterDistanciaLevenshtein(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return t.Length;
            if (string.IsNullOrEmpty(t)) return s.Length;

            var dp = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++)
                dp[i, 0] = i;
            for (int j = 0; j <= t.Length; j++)
                dp[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int custo = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(Math.Min(
                        dp[i - 1, j] + 1,      // exclusão
                        dp[i, j - 1] + 1),     // inserção
                        dp[i - 1, j - 1] + custo); // substituição
                }
            }

            return dp[s.Length, t.Length];
        }



    }
}
