using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Ionic.Zip;

namespace odict.ru
{
    public class FileBasedDictionary
    {
        public FileBasedDictionary (HttpServerUtility server)
        {
            this.server = server;
        }

        public void AddEntry (string entry)
        {
            File.AppendAllLines (ZalizniakFilePath, new [] { entry }, zalizniakFileEncoding);
        }

        public string ZalizniakFilePath { get { return CombinePath ("zalizniak.txt"); } }

        private string CombinePath (string filename)
        {
            return Path.Combine (server.MapPath ("~/App_Data"), filename);
        }

        private readonly HttpServerUtility server;
        private readonly Encoding zalizniakFileEncoding = Encoding.GetEncoding (1251);
        private const string forwardDawg = "forward.dawg";
        private const string reverseDawg = "reverse.dawg";
        private const string zipPath = "~/download/odict.zip";

        public void UpdateIndices ()
        {
            UpdateIndex (forwardDawg, DawgBuilder.BuildForward);
            UpdateIndex (reverseDawg, DawgBuilder.BuildReverse);
            UpdateZip ();
        }

        void UpdateIndex (string filename, Action <IEnumerable <KeyValuePair <string, string>>, string> rebuild)
        {
            UpdateFile (filename,
                        () =>
                        rebuild (
                            DawgBuilder.GetZalizniak (File.ReadAllLines (ZalizniakFilePath, zalizniakFileEncoding)),
                            CombinePath (filename)));
        }

        void UpdateFile (string filename, Action rebuild)
        {
            string filePath = CombinePath (filename);

            if (!File.Exists (filePath) || File.GetLastWriteTime (ZalizniakFilePath) > File.GetLastWriteTime (filePath))
            {
                new Task (rebuild).Start ();
            }
        }

        public Stream OpenForwardIndex ()
        {
            return OpenIndex (forwardDawg);
        }

        public Stream OpenReverseIndex ()
        {
            return OpenIndex (reverseDawg);
        }

        private Stream OpenIndex (string filename)
        {
            return SharedOpenDictionary(CombinePath (filename));
        }

        static Stream SharedOpenDictionary(string fullFileName)
        {
            return new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public bool FileExists ()
        {
            return File.Exists (ZalizniakFilePath);
        }

        public void UpdateZip ()
        {
            string zipFile = server.MapPath(zipPath);

            UpdateFile (zipFile, () => 
                {
                     using (var zip = new ZipFile())
                     {
                         zip.AddFile(ZalizniakFilePath, "");
                         zip.Save(zipFile);
                     }
                }
            );
        }
    }
}