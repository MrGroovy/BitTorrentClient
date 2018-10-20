using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace Lib.Bencoding.UnitTests
{
    [TestClass]
    public class BenValueParser_Integer_Tests
    {
        [TestInitialize]
        public void SetUp()
        {
        }

        [TestMethod]
        [DataRow("i1e", 1)]
        [DataRow("i62e", 62)]
        [DataRow("i480e", 480)]
        public void WhenStreamIsValidInteger_ThenIntegerIsCorrectlyParsed(string streamContent, int expectedValue)
        {
            // Arrange
            var data = Encoding.UTF8.GetBytes(streamContent);
            var memory = new MemoryStream(data);

            var parser = new BenValueParser(memory);

            // Act
            BenValue value = parser.Parse();

            // Assert
            Assert.IsInstanceOfType(value, typeof(BenInteger));
            Assert.AreEqual(expectedValue, ((BenInteger)value).Value);
        }
    }
}
