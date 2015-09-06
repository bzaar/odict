using NUnit.Framework;

namespace odict.ru.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1 ()
        {
            Assert.AreEqual("Анна,жо,Анны,Анне,Анну,Анной,Анне,Анне,Анны,Анн,Аннам,Анн,Аннами,Аннах,Аннах,Анною", 
                FileBasedDictionary.ExpandLine("Анна 1 жо 1а"));
        }

        [Ignore] [Test]
        public void GenerateCsv ()
        {
            FileBasedDictionary.GenerateCsv(@"..\..\..\..\zalizniak\zalizniak.txt", "odict.csv");
        }
    }
}
