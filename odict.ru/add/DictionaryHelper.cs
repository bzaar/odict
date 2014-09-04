using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace odict.ru.add
{
    public static class DictionaryHelper
    {
        public const string StressMark = "*";

        public static string RemoveStressMarks(string lemma)
        {
            return lemma.Replace(StressMark, String.Empty);
        }
    }
}