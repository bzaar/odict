using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DawgSharp;

namespace DawgBuilder
{
    class Program
    {
        static void Main()
        {
            BuildDictForSearch ();

            BuildModels ();
        }

        private static void BuildModels ()
        {
            var dict = new DawgBuilder <string> ();

            foreach (var entry in GetZalizniak ())
            {
                string lemma = entry.Key;

                dict.Insert (lemma.Reverse(), entry.Value);
            }

            var dawg = dict.BuildDawg ();

            dawg.SaveTo (File.Create (@"..\..\..\odict.ru\zalizniak.dawg"), (w, payload) => w.Write (payload ?? ""));
        }

        static IEnumerable <KeyValuePair <string, string>> GetZalizniak ()
        {
            var zalizniak = File.ReadAllLines (
                @"..\..\..\..\zalizniak\zalizniak.txt", // это тот же файл, что и odict.ru/download/odict.zip
                Encoding.GetEncoding (1251));

            foreach (var entry in zalizniak)
            {
                if (string.IsNullOrWhiteSpace (entry)) continue;

                if (entry.StartsWith ("-")) continue;

                int i = entry.IndexOf (' ');

                string lemma = entry.Substring (0, i);

                yield return new KeyValuePair <string, string> (lemma, entry.Substring (i + 1).Trim ());
            }
        }

        private static void BuildDictForSearch ()
        {
            var dict = new DawgBuilder <bool> ();

            foreach (var entry in GetZalizniak ())
            {
                string lemma = entry.Key;

                dict.Insert (lemma, true);
            }

            var dawg = dict.BuildDawg ();

            dawg.SaveTo (File.Create (@"..\..\..\odict.ru\dict.dawg"), (w, payload) => w.Write (payload));
        }
    }
}
