using System.Net.Mail;

namespace odict.ru
{
    class Email
    {
        public static void SendAdminEmail (string subject, string plainTextBody)
        {
            new SmtpClient ().Send (new MailMessage ("robot@odict.ru", "ss@odict.ru", subject, plainTextBody));
        }
    }
}