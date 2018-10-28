using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.UnitTests.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class PeerClient_ReceiveMessage_Tests
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
        public async Task WhenSocketReceivesZeroBytesBecauseEndOfStream_ThenAConnectionResetSocketExceptionIsThrown()
        {
            // Act/Assert
            var ex = await Assert.ThrowsExceptionAsync<SocketException>(() => client.ReceiveMessage());
            Assert.AreEqual(SocketError.ConnectionReset, ex.SocketErrorCode);
        }
    }
}
