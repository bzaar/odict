using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Ionic.Zip;
using Slepov.Russian;
using Zalizniak;

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

        public void UpdateIndices ()
        {
            UpdateForwardIndex ();
            UpdateReverseIndex ();
            UpdateZip ();
            UpdateWordFormsZip ();
        }

        public void UpdateForwardIndex ()
        {
            UpdateIndex (forwardDawg, DawgBuilder.BuildForward);
        }

        public void UpdateReverseIndex ()
        {
            UpdateIndex (reverseDawg, DawgBuilder.BuildReverse);
        }

        void UpdateIndex (string filename, Action <IEnumerable <KeyValuePair <string, string>>, string> rebuild)
        {
            UpdateFile (filename,
                        tmpFilePath =>
                        rebuild (
                            DawgBuilder.GetZalizniak (File.ReadAllLines (ZalizniakFilePath, zalizniakFileEncoding)),
                            tmpFilePath));
        }

        void UpdateFile (string filename, Action <string> rebuild)
        {
            string filePath = CombinePath (filename);

            string tmpFilePath = filePath + Guid.NewGuid () + ".tmp";

            if (!File.Exists (filePath) || File.GetLastWriteTime (ZalizniakFilePath) > File.GetLastWriteTime (filePath))
            {
                new Task (() => 
                {
                    try
                    {
                        rebuild (tmpFilePath);

                        MoveAndReplace (tmpFilePath, filePath);
                    }
                    catch (Exception e)
                    {
                        Email.SendAdminEmail ("File update failed: " + filename, e.ToString ());
                    }
                })
                .Start ();
            }
        }

        // .NET does not provide a File.MoveAndReplace method.
        // This tries to simulate such in an atomic way.
        // It may fail in a webfarm scenario (although chances are pretty slim).
        private static void MoveAndReplace (string srcFilePath, string dstFilePath)
        {
            lock (fileSystemLock)
            {
                if (File.Exists (dstFilePath))
                {
                    File.Delete (dstFilePath);
                }

                File.Move (srcFilePath, dstFilePath);
            }
        }

        static readonly object fileSystemLock = new object ();

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
            string zipFile = server.MapPath("~/download/odict.zip");

            UpdateFile (zipFile, new ZipArchive (ZalizniakFilePath).ZipSingleFile);
        }
        
        public void UpdateWordFormsZip ()
        {
            string zipFile = server.MapPath("~/download/wordforms.zip");
            string txtFile = server.MapPath("~/download/wordforms.txt");

            UpdateFile (zipFile, tmpFilePath => 
                {
                    var wordforms = File.ReadAllLines (ZalizniakFilePath, Encoding.GetEncoding (1251))
                        .AsParallel ()
                        .Select (line => FormGenerator.GetAccentedForms (line, delegate {}))
                        .SelectMany (forms => forms)
                        .Select (form => Stress.StripStressMarksAndYo (form.AccentedForm))
                        .OrderBy (form => form, StringComparer.Ordinal)
                        .Distinct ();
                            
                    File.WriteAllLines (txtFile, wordforms);

                    new ZipArchive (txtFile).ZipSingleFile (tmpFilePath);
                });
        }
    }

    class ZipArchive 
    {
        private readonly string filename;

        public ZipArchive (string filename)
        {
            this.filename = filename;
        }

        public void ZipSingleFile (string tmpFilePath)
        {
            using (var zip = new ZipFile())
            {
                zip.AddFile(filename, "");
                zip.Save(tmpFilePath);
            }
        }
    }
}