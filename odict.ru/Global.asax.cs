using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using System.IO;
using odict.ru.add;

namespace odict.ru
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            string AppDataPath = Server.MapPath("~/App_Data");
            if (!File.Exists(AppDataPath + "\\" + DawgHelper.DictionaryForSearchFileName) ||
                !File.Exists(AppDataPath + "\\" + DawgHelper.ModelsFileName))
            {
                DawgHelper.BuildDictionaries(AppDataPath);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}