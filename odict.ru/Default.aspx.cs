using System;
using System.Net.Mail;

namespace odict.ru
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DownloadButton_Click(object sender, EventArgs e)
        {
            DownloadFile("odict.zip");
        }

        protected void DownloadButton2_Click(object sender, EventArgs e)
        {
            DownloadFile("odict.csv.zip");
        }

        private void DownloadFile(string fileName)
        {
            Response.ContentType = "application/zip";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.WriteFile(Server.MapPath("~/download/" + fileName));

            Email.SendAdminEmail("oDict.ru: download " + fileName, "");
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            var mailMessage = new MailMessage
                                    {
                                        IsBodyHtml = true,
                                        Body = Resources.EmailTemplate,
                                        Subject = "Грамматический словарь русского языка"
                                    };

            string email = this.EmailTextBox.Text.Trim ();

            if (string.IsNullOrEmpty (email))
            {
                this.BadEmailLabel.Visible = true;
                return;
            }

            try
            {
                mailMessage.To.Add (email);

                new SmtpClient ().Send (mailMessage);
            }
            catch (Exception ex)
            {
                this.BadEmailLabel.Visible = true;
                this.BadEmailLabel.Text += " " + ex.Message;
                return;
            }

            this.SubscribedLabel.Visible = true;
            this.EmailTextBox.Text = string.Empty;

            Email.SendAdminEmail ("oDict.ru New Subscriber", email);

            using (var db = new DbContext ())
            {
                db.Replies.Add (new Reply
                {
                    Email = email,
                    DateTime = DateTime.UtcNow
                });

                db.SaveChanges ();
            }
        }
    }
}
