using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Globalization;

namespace odict.ru.add
{
    public static class DictionaryHelper
    {
        public const string StressMark = "*";

        public static readonly string[] Vowels = new string[] { "а", "е", "ё", "и", "о", "у", "ы", "э", "ю", "я" };

        //public static readonly Dictionary<string, string[]> Vowels = new Dictionary<string, string[]>()
        //{
        //    { "ru", new string[] { "а", "е", "ё", "и", "о", "у", "ы", "э", "ю", "я" } },
        //    { "uk", new string[] { } },
        //};

        public static bool CheckStreesPosition(string text)
        {
            int StressPosition = text.IndexOf(StressMark);

            return StressPosition > 0 // is strees mark specified and it's position after the first letter
                && Array.IndexOf(Vowels, text[StressPosition - 1].ToString()) != -1; //previous letter has to be a vowel
        }

        public static string RemoveStressMarks(string lemma)
        {
            return lemma.Replace(StressMark, String.Empty);
        }
    }
}