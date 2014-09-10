using System;
using System.Web.UI;

namespace odict.ru.add
{
    public partial class Default : Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.Browser.IsMobileDevice && Request.Browser.ScreenPixelsWidth < 800)
            {
                MasterPageFile = "~/Mobile.Master";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string NewLine = DictionaryHelper.RemoveStressMarks(lemma.Text) + " " + selectedRule.Text;
                int StressPos;
                string messageText;
                string messageStyle;

                if (String.IsNullOrEmpty(lemma.Text))
                {
                    messageText = "Введите слово.";
                    messageStyle = "messageError";
                }
                else if (!DictionaryHelper.CheckStressPosition (lemma.Text))
                {
                    messageText = "Укажите ударение. Например: приве*т.";
                    messageStyle = "messageError";
                }
                else if (!int.TryParse(selectedRule.Text.Substring(0, selectedRule.Text.IndexOfAny(" ,.".ToCharArray ())), out StressPos) || StressPos > DictionaryHelper.RemoveStressMarks(lemma.Text).Length)
                {
                    messageText = "Позиция ударения в правиле превышает длину слова.";
                    messageStyle = "messageError";
                }
                else
                {
                    messageText = "Статья \"" + NewLine + "\" добавлена в словарь.";
                    messageStyle = "messageSuccess";

                    lemma.Text = String.Empty;
                    selectedRule.Text = String.Empty;

                    var fileBasedDictionary = new FileBasedDictionary (Server);

                    fileBasedDictionary.AddEntry (NewLine);
                    fileBasedDictionary.UpdateIndices ();
                }

                message.Text = messageText;
                messageContainer.CssClass = "divMessage " + messageStyle; 
            }
        }
    }
}