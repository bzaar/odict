using System;
using System.Net.Mail;

namespace odict.ru
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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
            catch (Exception)
            {
                this.BadEmailLabel.Visible = true;
                return;
            }


            new SmtpClient ().Send (new MailMessage ("robot@odict.ru", "contact@morpher.ru", "oDict.ru Potential Customer", email));

            using (var db = new DbContext ())
            {
                db.Replies.Add (new Reply
                {
                    Email = email,
                    WantsToUse = this.UseCheckBox.Checked,
                    WantsToContribute = this.ContributeCheckBox.Checked,
                    WantsToBuy = this.PayCheckBox.Checked,
                    DateTime = DateTime.UtcNow
                });

                db.SaveChanges ();
            }

            Response.Redirect ("download/");
        }
    }
}
