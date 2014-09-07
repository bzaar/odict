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
                if (!String.IsNullOrEmpty(lemma.Text) && DictionaryHelper.CheckStressPosition(lemma.Text) &&
                    !String.IsNullOrEmpty(NewLine) && DawgHelper.AddItemToDictionary(Server.MapPath("~\\App_Data"), NewLine))
                {
                    message.Text = "Статья \"" + NewLine + "\" добавлена в словарь.";
                    message.CssClass = "messageSuccess";
                    lemma.Text = String.Empty;
                    selectedRule.Text = String.Empty;
                }
                else
                {
                    message.Text = "Ошибка при добавлении статьи \"" + NewLine + "\" в словарь!";
                    message.CssClass = "messageError";
                }
            }
        }
    }
}