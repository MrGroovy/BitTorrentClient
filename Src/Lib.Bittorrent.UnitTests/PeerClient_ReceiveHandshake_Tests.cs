﻿using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.UnitTests.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class PeerClient_ReceiveHandshake_Tests
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
        public async Task WhenPeerIdDoesNotDecodeToStringOfLength20_ThenPeerIsHandledCorrectly()
        {
            // Arrange
            byte[] peerId = new byte[] {
                45, 108, 116, 48, 68,
                50, 48, 45, 89, 218,
                26, 145, 226, 9, 87,
                63, 146, 213, 165, 248 };
            socket.SetUpHandshake(peerId: peerId);

            // Act
            ProtocolMessage msg = await client.ReceiveHandshakeMessage();

            // Assert
            CollectionAssert.AreEqual(peerId, ((HandshakeMessage)msg).PeerId);
        }
    }
}
