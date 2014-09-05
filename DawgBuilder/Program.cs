using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DawgSharp;

namespace DawgBuilder
{
    class Program
    {
        private static string DictionaryForSearchFileName = "DictSearch.dawg";
        private static string ModelsFileName = "DictModels.dawg";

        private static void WriteToLog(StreamWriter writer, string text)
        {
            writer.WriteLine("[{0}] {1}", DateTime.Now, text);
            writer.Flush();
        }

        static void Main(string[] args)
        {            
            string LogFileName = Path.ChangeExtension(System.Reflection.Assembly.GetExecutingAssembly().Location, ".log");
            StreamWriter Logger = new StreamWriter(new FileStream(LogFileName, FileMode.Append, FileAccess.Write));

            if (args.Length < 2)
            {
                WriteToLog(Logger, "Not enought parameters!");
                Logger.Close();
                return;
            }

            string InputFileName = args[0];
            string OutputDirectory = args[1];

            IEnumerable<KeyValuePair<string, string>> Zalizniak = null;
            try
            {
                Zalizniak = GetZalizniak(InputFileName);
            }
            catch (Exception exp)
            {
                WriteToLog(Logger, "Couldn't got Zalizniak! Message: " + exp.Message);
                Logger.Close();
                return;
            }

            string DictSearchOutputFileName = OutputDirectory + "\\" + DictionaryForSearchFileName;
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
            } 

            if (!ErrorBuildForSearch)
            {
                WriteToLog(Logger, "Dictionary for search had built successfully.");
            }

            string ModelsOutputFileName = OutputDirectory + "\\" + ModelsFileName;
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
            }

            if (!ErrorBuildModels)
            {
                WriteToLog(Logger, "Models had built successfully.");
            }


            Logger.Close();
        }

        private static void BuildModels(IEnumerable<KeyValuePair<string, string>> zalizniak, string outputFileName)
        {
            using (FileStream ModelsFile = File.Create(outputFileName))
            {
                var dict = new DawgBuilder <string> ();

                foreach (var entry in zalizniak)
                {
                    string lemma = entry.Key;

                    dict.Insert (lemma.Reverse(), entry.Value);
                }

                var dawg = dict.BuildDawg ();

                dawg.SaveTo(ModelsFile, (w, payload) => w.Write(payload ?? ""));
            }
        }

        static IEnumerable<KeyValuePair<string, string>> GetZalizniak(string inputFileName)
        {
            var zalizniak = File.ReadAllLines (inputFileName, Encoding.GetEncoding (1251));

            foreach (var entry in zalizniak)
            {
                if (string.IsNullOrWhiteSpace (entry)) continue;

                if (entry.StartsWith ("-")) continue;

                int i = entry.IndexOf (' ');

                string lemma = entry.Substring (0, i);

                yield return new KeyValuePair <string, string> (lemma, entry.Substring (i + 1).Trim ());
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
