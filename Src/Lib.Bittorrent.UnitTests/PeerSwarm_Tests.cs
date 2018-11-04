using Lib.Bittorrent.Messages;
using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class PeerSwarm_Tests
    {
        private Mock<IPeerClient> peerClient;
        private Mock<IPeerClientFactory> peerClientFactory;
        private Mock<IMessageLoop> loop;
        private PeerSwarm swarm;

        [TestInitialize]
        public void SetUp()
        {
            peerClient = new Mock<IPeerClient>();
            peerClient
                .Setup(m => m.ReceiveHandshakeMessage())
                .ReturnsAsync(new HandshakeMessage(
                    "BitTorrent protocol",
                    new byte[8],
                    new byte[20],
                    new byte[20]));
            peerClient.SetupGet(m => m.Ip).Returns(IPAddress.Loopback);
            peerClient.SetupGet(m => m.Port).Returns(6881);

            peerClientFactory = new Mock<IPeerClientFactory>();
            peerClientFactory
                .Setup(m => m.Create())
                .Returns(() => peerClient.Object);

            loop = new Mock<IMessageLoop>();

            swarm = new PeerSwarm(
                loop.Object,
                peerClientFactory.Object,
                Mock.Of<ILogger<PeerSwarm>>());
        }

        [TestMethod]
        public async Task WhenPeerClientReceivesBitfieldMessage_ThenItIsPropagatedToTheLoop()
        {
            // Arrange
            var bitfield = new BitfieldMessage(new byte[1] { 0b10101111 });
            peerClient
                .SetupSequence(m => m.ReceiveMessage())
                .ReturnsAsync(bitfield);

            // Act
            await swarm.Connect(IPAddress.Loopback, 6881, new byte[20]);
            await swarm.Close(IPAddress.Loopback, 6881);

            // Assert
            loop.Verify(m => m.PostBitfieldReceivedEvent(
                IPAddress.Loopback,
                6881,
                It.Is<BitfieldMessage>(b => ReferenceEquals(b, bitfield))));
        }
    }
}
