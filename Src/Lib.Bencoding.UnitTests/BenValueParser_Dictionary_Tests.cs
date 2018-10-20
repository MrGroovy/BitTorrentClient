using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text;

namespace Lib.Bencoding.UnitTests
{
    [TestClass]
    public class BenValueParser_Dictionary_Tests
    {
        [TestInitialize]
        public void SetUp()
        {
        }

        [TestMethod]
        public void WhenStreamIsValidDictionary_ThenDictionaryIsCorrectlyParsed()
        {
            // Arrange
            var data = Encoding.UTF8.GetBytes("d3:Fooi42ee");
            var memory = new MemoryStream(data);

            var parser = new BenValueParser(memory);

            // Act
            BenValue result = parser.Parse();

            // Assert
            BenDictionary dict = (BenDictionary)result;

            BenByteString key = dict.Values.Keys.First();
            BenInteger value = (BenInteger)dict.Values.Values.First();

            Assert.AreEqual("Foo", Encoding.UTF8.GetString(key.Value));
            Assert.AreEqual(42, value.Value);
        }
    }
}
