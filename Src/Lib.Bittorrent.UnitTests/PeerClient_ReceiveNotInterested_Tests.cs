using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.UnitTests.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class PeerClient_ReceiveNotInterested_Tests
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
        public async Task WhenNotInterestedBytesAreReceivedFromSocket_ThenNotInterestedMessageIsReturned()
        {
            // Arrange
            socket.SetUpNotInterested();
            
            // Act
            ProtocolMessage msg = await client.ReceiveMessage();

            // Assert
            Assert.IsInstanceOfType(msg, typeof(NotInterestedMessage));
        }
    }
}
