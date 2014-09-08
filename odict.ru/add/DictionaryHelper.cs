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
        public const string RuleLineDelimiter = "=";

        public static bool CheckStressPosition(string text)
        {
            int StressPosition = text.IndexOf(StressMark);

            return StressPosition > 0 // is strees mark specified and it's position after the first letter
                && Slepov.Russian.Syllable.LowercaseVowels.IndexOf (char.ToLowerInvariant (text[StressPosition - 1])) != -1; //previous letter has to be a vowel
        }

        public static string RemoveStressMarks(string lemma)
        {
            return lemma.Replace(StressMark, String.Empty);
        }
    }
}