using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.UnitTests.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class PeerClient_ReceiveHave_Tests
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
        public async Task WhenHaveMessageIsReceived_ThenItHasCorrectPieceIndex()
        {
            // Arrange
            socket.SetUpTestData(
                new byte[] { 0, 0, 0, 5 },
                new byte[] { 4 },
                new byte[] { 0, 0, 0, 64 });

            // Act
            ProtocolMessage msg = await client.ReceiveMessage();

            // Assert
            var haveMsg = (HaveMessage)msg;
            Assert.AreEqual(64, haveMsg.PieceIndex);
        }
    }
}
