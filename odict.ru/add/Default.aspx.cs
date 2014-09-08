using System;
using System.Web.UI;

namespace odict.ru.add
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string NewLine = DictionaryHelper.RemoveStressMarks(lemma.Text) + " " + selectedRule.Text;
                int StressPos = -1;

                if (String.IsNullOrEmpty(lemma.Text))
                {
                    message.Text = "Введите слово.";
                    message.CssClass = "messageError";
                }
                else if (!DictionaryHelper.CheckStressPosition (lemma.Text))
                {
                    message.Text = "Укажите ударение. Например: приве*т.";
                    message.CssClass = "messageError";
                }
                else if (!int.TryParse(selectedRule.Text.Substring(0, selectedRule.Text.IndexOf(" ")), out StressPos) || StressPos > DictionaryHelper.RemoveStressMarks(lemma.Text).Length)
                {
                    message.Text = "Позиция ударения в правиле превышает длину слова.";
                    message.CssClass = "messageError";
                }
                else
                {
                    message.Text = "Статья \"" + NewLine + "\" добавлена в словарь.";
                    message.CssClass = "messageSuccess";

                    lemma.Text = String.Empty;
                    selectedRule.Text = String.Empty;

                    var fileBasedDictionary = new FileBasedDictionary (Server);

                    fileBasedDictionary.AddEntry (NewLine);
                    fileBasedDictionary.UpdateIndices ();
                }
            }
        }
    }
}