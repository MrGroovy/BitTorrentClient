using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace Lib.Bencoding.UnitTests
{
    [TestClass]
    public class BenValueParser_ByteString_Tests
    {
        [TestInitialize]
        public void SetUp()
        {
        }

        [TestMethod]
        [DataRow("1:a", "a")]
        [DataRow("5:Hello", "Hello")]
        [DataRow("2::x", ":x")]
        public void WhenStreamIsValidByteString_ThenByteStringIsCorrectlyParsed(string streamContent, string expectedValue)
        {
            // Arrange
            var data = Encoding.UTF8.GetBytes(streamContent);
            var memory = new MemoryStream(data);

            var parser = new BenValueParser(memory);

            // Act
            BenValue value = parser.Parse();

            // Assert
            BenByteString result = (BenByteString)value;

            Assert.AreEqual(expectedValue, Encoding.UTF8.GetString(result.Value));
        }
    }
}
