using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace odict.ru.add
{
    public partial class Default : Page
    {
        private static readonly object DawgLocker = new object();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string NewLine = DictionaryHelper.RemoveStressMarks(lemma.Text) + " " + selectedRule.Text;
                if (!String.IsNullOrEmpty(lemma.Text) && DictionaryHelper.CheckStreesPosition(lemma.Text) &&
                    !String.IsNullOrEmpty(NewLine) && DawgHelper.AddItemToDictionary(Server.MapPath("~\\App_Data"), NewLine))
                {
                    message.Text = "Статья &quot;" + NewLine + "&quot; добавлена в словарь.";
                    message.CssClass = "messageSuccess";
                    lemma.Text = String.Empty;
                    selectedRule.Text = String.Empty;
                }
                else
                {
                    message.Text = "Ошибка при добавлении статьи &quot;" + NewLine + "&quot; в словарь!";
                    message.CssClass = "messageError";
                }
            }
        }
    }
}