﻿using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.UnitTests.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class PeerClient_ReceiveChoke_Tests
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
        public async Task WhenChokeBytesAreReceivedFromSocket_ThenChokeMessageIsReturned()
        {
            // Arrange
            socket.SetUpChoke();
            
            // Act
            ProtocolMessage msg = await client.ReceiveMessage();

            // Assert
            Assert.IsInstanceOfType(msg, typeof(ChokeMessage));
        }
    }
}
