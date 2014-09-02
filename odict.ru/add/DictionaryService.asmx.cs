using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using odict.ru.add;

namespace odict.ru
{
    /// <summary>
    /// Summary description for DictionaryService
    /// </summary>
    [WebService (Namespace = "http://odict.ru/")]
    [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem (false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class DictionaryService : System.Web.Services.WebService
    {

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public string [] GetCompletionList (string prefixText, int count)
        {
            var dict = DawgSharp.Dawg<bool>.Load (new MemoryStream (Resources.dict), r => r.ReadBoolean ());

            return dict.MatchPrefix (Default.RemoveStressMarks (prefixText)).Take (10).Select (kvp => kvp.Key).ToArray ();
        }
    }
}
