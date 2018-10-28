using Lib.Bittorrent.Messages;
using Lib.Bittorrent.Messages.Events;
using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class ReceiveErrorEvent_Tests
    {
        private TorrentState state;
        private Mock<IPeerSwarm> swarm;
        private Mock<IMessageLoop> loop;

        [TestInitialize]
        public void SetUp()
        {
            state = new TorrentState(Mock.Of<ILogger<TorrentState>>());
            swarm = new Mock<IPeerSwarm>();
            loop = new Mock<IMessageLoop>();
        }

        [TestMethod]
        public async Task WhenExecutingReceiveErrorEvent_ThenThePeerIsRemovedFromTheSwarm()
        {
            // Arrange
            var receiveError = new ReceiveErrorEvent(
                IPAddress.Loopback,
                6881,
                state,
                swarm.Object);

            // Act
            await receiveError.Execute(loop.Object);

            // Assert
            swarm.Verify(m => m.Remove(IPAddress.Loopback, 6881));
        }
    }
}
