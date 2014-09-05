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
                if (!String.IsNullOrEmpty(lemma.Text) && lemma.Text.IndexOf(DictionaryHelper.StressMark) != -1 &&
                    !String.IsNullOrEmpty(selectedRule.Value)) // or selectedRule.Value.IndexOf("?") == -1
                {
                    bool Result = DawgHelper.AddItemToDictionary(Server.MapPath("~\\App_Data"), selectedRule.Value);
                        
                    if (Result)
                    {
                        message.Text = "Строка " + selectedRule.Value + " успешно добавлена в словарь!";
                        message.CssClass = "messageSuccess";
                        lemma.Text = String.Empty;
                        selectedRule.Value = String.Empty;
                    }
                    else
                    {
                        message.Text = "Ошибка при добавлении строки " + selectedRule.Value + " в словарь!";
                        message.CssClass = "messageError";
                    }
                }
            }
        }
    }
}