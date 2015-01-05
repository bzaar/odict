using System;
using System.Web;

namespace odict.ru
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var dictionary = new FileBasedDictionary (Server);

            if (!dictionary.FileExists ())
            {
                throw new Exception ("App_Data/zalizniak.txt is missing.");
            }

            dictionary.UpdateIndices ();

            new SchedulerTask (dictionary.UpdateFiles, TimeSpan.FromMinutes (2), TimeSpan.FromMinutes (1));
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