using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace Lib.Bencoding.UnitTests
{
    [TestClass]
    public class BenValueParser_List_Tests
    {
        [TestInitialize]
        public void SetUp()
        {
        }

        [TestMethod]
        public void WhenStreamIsValidList_ThenListIsCorrectlyParsed()
        {
            // Arrange
            var data = Encoding.UTF8.GetBytes("l3:Fooi42ee");
            var memory = new MemoryStream(data);

            var parser = new BenValueParser(memory);

            // Act
            BenValue value = parser.Parse();

            // Assert
            BenList result = (BenList)value;

            BenByteString elementOne = (BenByteString)result.Values[0];
            BenInteger elementTwo = (BenInteger)result.Values[1];

            Assert.AreEqual("Foo", Encoding.UTF8.GetString(elementOne.Value));
            Assert.AreEqual(42, elementTwo.Value);
        }

        [TestMethod]
        public void WhenListInsideList_ThenListIsCorrectlyParsed()
        {
            // Arrange
            var data = Encoding.UTF8.GetBytes("ll3:Fooee");
            var memory = new MemoryStream(data);

            var parser = new BenValueParser(memory);

            // Act
            BenValue value = parser.Parse();

            // Assert
            BenList result = (BenList)value;
            BenList innerList = (BenList)result.Values[0];
            BenByteString elementTwo = (BenByteString)innerList.Values[0];

            Assert.AreEqual("Foo", Encoding.UTF8.GetString(elementTwo.Value));
        }
    }
}
