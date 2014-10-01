using System.Diagnostics;
using System.Net.Mail;

namespace odict.ru
{
    class Email
    {
        public static void SendAdminEmail (string subject, string plainTextBody)
        {
            if (!Debugger.IsAttached)
            {
                new SmtpClient ().Send (new MailMessage ("robot@odict.ru", "ss@odict.ru", subject, plainTextBody));
            }
        }
    }
}