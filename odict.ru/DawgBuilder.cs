using System.Collections.Generic;
using System.Linq;
using System.IO;
using DawgSharp;

namespace odict.ru
{
    static class DawgBuilder
    {
        public static void BuildReverse(IEnumerable<KeyValuePair<string, string>> zalizniak, string outputFileName)
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

        public static void BuildForward(IEnumerable<KeyValuePair<string, string>> zalizniak, string outputFileName)
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

        public static IEnumerable<KeyValuePair<string, string>> GetZalizniak(IEnumerable<string> lines)
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
    }
}