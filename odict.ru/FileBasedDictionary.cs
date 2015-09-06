using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Ionic.Zip;
using Slepov.Russian;
using Slepov.Russian.Зализняк;
using Zalizniak;

namespace odict.ru
{
    public class FileBasedDictionary
    {
        public FileBasedDictionary (HttpServerUtility server)
        {
            this.server = server;

            this.filesToRebuild = new Dictionary <string, Action> {
                {forwardDawgAppRelativePath, UpdateForwardIndex},
                {reverseDawgAppRelativePath, UpdateReverseIndex},
                {odictZipAppRelativePath, UpdateZip},
                {wordformsZipAppRelativePath, UpdateWordFormsZip},
                {csvZipAppRelativePath, UpdateCsvZip}
            };
        }

        readonly Dictionary <string, Action> filesToRebuild;

        public void AddEntry (string entry)
        {
            File.AppendAllLines (ZalizniakFilePath, new [] { entry }, zalizniakFileEncoding);
        }

        string ZalizniakFilePath { get { return MapPath ("~/App_Data/zalizniak.txt"); } }

        private string MapPath (string appRelativePath)
        {
            return server.MapPath (appRelativePath);
        }

        private readonly HttpServerUtility server;
        private readonly Encoding zalizniakFileEncoding = Encoding.GetEncoding (1251);

        private const string forwardDawgAppRelativePath  = "~/App_Data/forward.dawg";
        private const string reverseDawgAppRelativePath  = "~/App_Data/reverse.dawg";
        private const string wordformsZipAppRelativePath = "~/download/wordforms.zip";
        private const string odictZipAppRelativePath     = "~/download/odict.zip";
        private const string csvZipAppRelativePath       = "~/download/odict.csv.zip";

        public void UpdateIndices ()
        {
            UpdateForwardIndex ();
            UpdateReverseIndex ();
        }

        void UpdateForwardIndex ()
        {
            UpdateIndex (forwardDawgAppRelativePath, DawgBuilder.BuildForward);
        }

        void UpdateReverseIndex ()
        {
            UpdateIndex (reverseDawgAppRelativePath, DawgBuilder.BuildReverse);
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
            string filePath = MapPath (filename);

            string tmpFilePath = filePath + Guid.NewGuid () + ".tmp";

            if (!File.Exists (filePath) || File.GetLastWriteTime (ZalizniakFilePath) > File.GetLastWriteTime (filePath))
            {
                rebuild (tmpFilePath);

                MoveAndReplace (tmpFilePath, filePath);
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
            return OpenIndex (forwardDawgAppRelativePath);
        }

        public Stream OpenReverseIndex ()
        {
            return OpenIndex (reverseDawgAppRelativePath);
        }

        private Stream OpenIndex (string appRelativeFilePath)
        {
            return SharedOpenDictionary(MapPath (appRelativeFilePath));
        }

        static Stream SharedOpenDictionary(string fullFileName)
        {
            return new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public bool FileExists ()
        {
            return File.Exists (ZalizniakFilePath);
        }

        void UpdateZip ()
        {
            UpdateFile (odictZipAppRelativePath, new ZipArchive (ZalizniakFilePath).ZipSingleFile);
        }
        
        void UpdateWordFormsZip ()
        {
            string txtFile = server.MapPath("~/download/wordforms.txt");

            UpdateFile (wordformsZipAppRelativePath, tmpFilePath => 
                {
                    var wordforms = File.ReadAllLines (ZalizniakFilePath, Encoding.GetEncoding (1251))
                        .AsParallel ()
                        .Select (line => FormGenerator.GetAccentedForms (line, delegate {}).ToArray())
                        .SelectMany (forms => forms)
                        .Select (form => Stress.StripStressMarksAndYo (form.AccentedForm))
                        .OrderBy (form => form, StringComparer.Ordinal)
                        .Distinct ();
                            
                    File.WriteAllLines (txtFile, wordforms);

                    new ZipArchive (txtFile).ZipSingleFile (tmpFilePath);
                });
        }

        public void UpdateCsvZip ()
        {
            string txtFile = server.MapPath("~/download/odict.csv");

            UpdateFile (csvZipAppRelativePath, tmpFilePath =>
                                                   {
                                                       GenerateCsv(ZalizniakFilePath, txtFile);

                                                       new ZipArchive (txtFile).ZipSingleFile (tmpFilePath);
                                                   });
        }

        internal static void GenerateCsv(string zalizniakFilePath, string outputFileName)
        {
            var wordforms = File.ReadAllLines (zalizniakFilePath, Encoding.GetEncoding (1251))
                .AsParallel ()
                .Where(line => !string.IsNullOrEmpty(line))
                .Select (ExpandLine)
                .Where (line => line != null)
                .OrderBy (line => line);
                            
            File.WriteAllLines (outputFileName, wordforms, Encoding.GetEncoding(1251));
        }

        internal static string ExpandLine(string line)
        {
            var obj = new EntryParser(line).Parse();

            var entry = obj as Статья;

            if (entry == null) return null;

            string lemma;
            string symbol;
            int[] secAccPos;
            new EntryParser(line).ParseCommonPart(out lemma, out symbol, out secAccPos);

            bool failed = false;
            var forms = FormGenerator.GetAccentedFormsWithCorrectCase (line, delegate { failed = true; });

            if (failed) return null;

            return lemma + "," + symbol + "," + string.Join(",", forms.Skip(1)
                .Select (form => Stress.StripStressMarks (form.AccentedForm)));
        }

        public void UpdateFiles ()
        {
            foreach (var file in filesToRebuild)
            {
                string fullFilePath = MapPath (file.Key);

                var fileTime = File.Exists (fullFilePath) ? File.GetLastWriteTime (fullFilePath) : (DateTime.Now - TimeSpan.FromDays (1));

                var zalFileTime = File.GetLastWriteTime (ZalizniakFilePath);

                var diff = zalFileTime - fileTime;

                var now = DateTime.Now;

                if (diff > TimeSpan.FromHours (1) || (diff > TimeSpan.FromMinutes (5) && (now - zalFileTime) > TimeSpan.FromMinutes (5)))
                {
                    try
                    {
                        Email.SendAdminEmail ("Updating " + file.Key, "Diff: " + diff);

                        file.Value (); // go update the file

                        // Set the time stamp for the generated file to the same as the source file.
                        File.SetLastWriteTime (fullFilePath, zalFileTime);
                    }
                    catch (Exception e)
                    {
                        Email.SendAdminEmail ("File update failed: " + file.Key, e.ToString ());
                    }
                }
            }
        }
    }

    class ZipArchive 
    {
        private readonly string filename;

        public ZipArchive (string filename)
        {
            this.filename = filename;
        }

        public void ZipSingleFile (string zipFilePath)
        {
            using (var zip = new ZipFile())
            {
                zip.AddFile(filename, "");
                zip.Save(zipFilePath);
            }
        }
    }
}