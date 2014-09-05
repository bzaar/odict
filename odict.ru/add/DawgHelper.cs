using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using Zalizniak;
using DawgSharp;
using System.Text;

namespace odict.ru.add
{
    public static class DawgHelper
    {
        public const string BaseDictionaryFileName = "zalizniak.txt";
        public const string DictionaryForSearchFileName = "forward.dawg";
        public const string ModelsFileName = "reverse.dawg";

        private const string LockFlagFileName = "lock.lck";
        private const string LogFileName = "DawgHelper.log";

        private static readonly TimeSpan LockFlagFileTTL = TimeSpan.FromSeconds(30);

        private static readonly Encoding InputFileEncoding = Encoding.GetEncoding(1251);

        private static void WriteToLog(StreamWriter writer, string text)
        {
            writer.WriteLine("[{0}] {1}", DateTime.Now, text);
            writer.Flush();
        }

        private static bool BuildDictionaries(string dataDirectory, string lemma)
        {
            StreamWriter Logger = new StreamWriter(new FileStream(dataDirectory + "\\" + LogFileName, FileMode.Append, FileAccess.Write));

            string BaseDictionaryFullFileName = dataDirectory + "\\" + BaseDictionaryFileName;
            string LockFlagFullFileName = dataDirectory + "\\" + LockFlagFileName;
            
            if (File.Exists(LockFlagFullFileName))
            {
                if (new FileInfo(LockFlagFullFileName).CreationTime < DateTime.Now.AddMilliseconds(-LockFlagFileTTL.TotalMilliseconds))
                {
                    File.Delete(LockFlagFullFileName);
                }
                else
                {
                    WriteToLog(Logger, "Operation locked!");
                    Logger.Close();
                    return false;
                }
            }

            File.WriteAllText(LockFlagFullFileName, String.Empty);

            IEnumerable<KeyValuePair<string, string>> Zalizniak = null;
            try
            {
                List<string> InputLines = new List<string>(File.ReadAllLines(BaseDictionaryFullFileName, InputFileEncoding));
                if (!String.IsNullOrEmpty(lemma))
                {
                    InputLines.Add(lemma);
                    File.WriteAllLines(BaseDictionaryFullFileName, InputLines, InputFileEncoding);
                    WriteToLog(Logger, "Added line to base file -> " + lemma);
                }

                Zalizniak = GetZalizniak(InputLines);
            }
            catch (Exception exp)
            {
                WriteToLog(Logger, "Couldn't open or write base file! Message: " + exp.Message);
                Logger.Close();
                File.Delete(LockFlagFullFileName);
                return false;
            }

            string DictSearchOutputFileName = dataDirectory + "\\" + DictionaryForSearchFileName;
            WriteToLog(Logger, "Starting to build dictionary for search -> " + DictSearchOutputFileName);
            bool ErrorBuildForSearch = false;
            try
            {
                BuildDictForSearch(Zalizniak, DictSearchOutputFileName);
            }
            catch (Exception exp)
            {
                WriteToLog(Logger, "Couldn't build dictionary for search! Message: " + exp.Message);
                ErrorBuildForSearch = true;
                //Logger.Close();
                //return false;
            }

            if (!ErrorBuildForSearch)
            {
                WriteToLog(Logger, "Dictionary for search had built successfully.");
            }

            string ModelsOutputFileName = dataDirectory + "\\" + ModelsFileName;
            WriteToLog(Logger, "Starting to build models -> " + ModelsOutputFileName);
            bool ErrorBuildModels = false;
            try
            {
                BuildModels(Zalizniak, ModelsOutputFileName);
            }
            catch (Exception exp)
            {
                WriteToLog(Logger, "Couldn't build models! Message: " + exp.Message);
                ErrorBuildModels = true;
                //Logger.Close();
                //return false;
            }

            if (!ErrorBuildModels)
            {
                WriteToLog(Logger, "Models had built successfully.");
            }


            Logger.Close();
            File.Delete(LockFlagFullFileName);
            
            return !ErrorBuildModels;
        }

        public static bool BuildDictionaries(string dataDirectory)
        {
            return BuildDictionaries(dataDirectory, null);
        }

        public static bool AddItemToDictionary(string dataDirectory, string lemma)
        {
            return BuildDictionaries(dataDirectory, lemma);
        }

        public static Stream SharedOpenDictionary(string fullFileName)
        {
            return new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private static void BuildModels(IEnumerable<KeyValuePair<string, string>> zalizniak, string outputFileName)
        {
            using (FileStream ModelsFile = File.Create(outputFileName))
            {
                var dict = new DawgBuilder<string>();

                foreach (var entry in zalizniak)
                {
                    string lemma = entry.Key;

                    dict.Insert(lemma.Reverse(), entry.Value);
                }

                var dawg = dict.BuildDawg();

                dawg.SaveTo(ModelsFile, (w, payload) => w.Write(payload ?? ""));
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetZalizniak(IEnumerable<string> lines)
        {
            foreach (var entry in lines)
            {
                if (string.IsNullOrWhiteSpace(entry)) continue;

                if (entry.StartsWith("-")) continue;

                int i = entry.IndexOf(' ');

                string lemma = entry.Substring(0, i);

                yield return new KeyValuePair<string, string>(lemma, entry.Substring(i + 1).Trim());
            }
        }

        private static void BuildDictForSearch(IEnumerable<KeyValuePair<string, string>> zalizniak, string outputFileName)
        {
            using (FileStream DictSearchFile = File.Create(outputFileName))
            {
                var dict = new DawgBuilder<bool>();

                foreach (var entry in zalizniak)
                {
                    string lemma = entry.Key;

                    dict.Insert(lemma, true);
                }

                var dawg = dict.BuildDawg();

                dawg.SaveTo(DictSearchFile, (w, payload) => w.Write(payload));
            }
        }
    }
}