using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.UnitTests.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class PeerClient_ReceiveBitfield_Tests
    {
        private FakeSocket socket;
        private PeerClient client;

        [TestInitialize]
        public void SetUp()
        {
            socket = new FakeSocket();
            client = new PeerClient(
                socket,
                Mock.Of<ILogger<PeerClient>>());
        }

        [TestMethod]
        public async Task WhenBitfieldMessageWithOneByteIsReceived_ThenItHasCorrectBits()
        {
            // Arrange
            socket.SetUpTestData(
                new byte[] { 0, 0, 0, 2 },
                new byte[] { 5 },
                new byte[] { 0b11001000 });

            // Act
            ProtocolMessage msg = await client.ReceiveMessage();

            // Assert
            var bitfieldMsg = (BitfieldMessage)msg;

            Assert.IsTrue(bitfieldMsg.Bits[0]);
            Assert.IsTrue(bitfieldMsg.Bits[1]);
            Assert.IsFalse(bitfieldMsg.Bits[2]);
            Assert.IsFalse(bitfieldMsg.Bits[3]);

            Assert.IsTrue(bitfieldMsg.Bits[4]);
            Assert.IsFalse(bitfieldMsg.Bits[5]);
            Assert.IsFalse(bitfieldMsg.Bits[6]);
            Assert.IsFalse(bitfieldMsg.Bits[7]);
        }

        [TestMethod]
        public async Task WhenBitfieldMessageWithTwoBytesIsReceived_ThenItHasCorrectBits()
        {
            // Arrange
            socket.SetUpTestData(
                new byte[] { 0, 0, 0, 3 },
                new byte[] { 5 },
                new byte[] { 0b11001000, 0b00101111 });

            // Act
            ProtocolMessage msg = await client.ReceiveMessage();

            // Assert
            var bitfieldMsg = (BitfieldMessage)msg;

            Assert.IsTrue(bitfieldMsg.Bits[0]);
            Assert.IsTrue(bitfieldMsg.Bits[1]);
            Assert.IsFalse(bitfieldMsg.Bits[2]);
            Assert.IsFalse(bitfieldMsg.Bits[3]);

            Assert.IsTrue(bitfieldMsg.Bits[4]);
            Assert.IsFalse(bitfieldMsg.Bits[5]);
            Assert.IsFalse(bitfieldMsg.Bits[6]);
            Assert.IsFalse(bitfieldMsg.Bits[7]);

            Assert.IsFalse(bitfieldMsg.Bits[8]);
            Assert.IsFalse(bitfieldMsg.Bits[9]);
            Assert.IsTrue(bitfieldMsg.Bits[10]);
            Assert.IsFalse(bitfieldMsg.Bits[11]);

            Assert.IsTrue(bitfieldMsg.Bits[12]);
            Assert.IsTrue(bitfieldMsg.Bits[13]);
            Assert.IsTrue(bitfieldMsg.Bits[14]);
            Assert.IsTrue(bitfieldMsg.Bits[15]);
        }
    }
}
