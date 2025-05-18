using System.Net;
using System.Net.Mail;

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
    }
}
