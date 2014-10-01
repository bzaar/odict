using System;
using System.IO;
using System.Linq;
using System.Web.Services;

namespace odict.ru.add
{
    /// <summary>
    /// Summary description for DictionaryService
    /// </summary>
    [WebService (Namespace = "http://odict.ru/")]
    [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem (false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class DictionaryService : WebService
    {
        [WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public string [] GetCompletionList (string prefixText, int count)
        {
            var fileBasedDictionary = new FileBasedDictionary(Server);

            try
            {
                using (Stream ForwardDict = fileBasedDictionary.OpenForwardIndex())
                {
                    var dict = DawgSharp.Dawg<bool>.Load(ForwardDict, r => r.ReadBoolean());

                    return dict.MatchPrefix(DictionaryHelper.RemoveStressMarks(prefixText).ToLowerInvariant()).Take(10).Select(kvp => kvp.Key).ToArray();
                }
            }
            catch (Exception exp)
            {
                fileBasedDictionary.UpdateForwardIndex();

                Email.SendAdminEmail ("GetCompletionList", exp.ToString ());

                return new string[] { "Доступ к словарю в данный момент отсутствует. Возможно происходит построение индексов." };
            }
        }
    }
}
