using Lib.Bittorrent.Messages;
using Lib.Bittorrent.MetainfoDecoding;
using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Tracker.Client;
using Lib.Bittorrent.Tracker.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    [DeploymentItem(@"TorrentFiles")]
    public class CallTracker_Tests
    {
        private TorrentState state;
        private Mock<ITrackerClient> trackerClient;
        private Mock<IMessageLoop> messageLoop;
        private CallTracker callTracker;

        [TestInitialize]
        public void SetUp()
        {
            state = new TorrentState(Mock.Of<ILogger<TorrentState>>());
            state.MetaInfo = new MetaInfoSerializer()
                .Deserialize(
                    new MemoryStream(
                        File.ReadAllBytes(@"TorrentFiles\debian-9.4.0-amd64-netinst.iso.torrent")));
            trackerClient = new Mock<ITrackerClient>();
            messageLoop = new Mock<IMessageLoop>();
            callTracker = new CallTracker(
                trackerClient.Object,
                state,
                Mock.Of<ILogger<CallTracker>>());
        }

        [TestMethod]
        public async Task WhenResponseHasOnePeer_ThenItIsAddedToTheState()
        {
            // Arrange
            trackerClient
                .Setup(m => m.CallTracker(It.IsAny<string>(), It.IsAny<TrackerRequestDto>()))
                .ReturnsAsync(new TrackerResponseDto
                {
                    Interval = 200,
                    Peers =
                    {
                        new PeerDto
                        {
                            Ip = IPAddress.Parse("10.183.94.41"),
                            Port = 6881,
                            PeerId = new byte[20]
                        }
                    }
                });

            // Act
            await callTracker.Execute(messageLoop.Object);

            // Assert
            Assert.IsTrue(state.Peers.Any(p =>
                p.Ip.Equals(IPAddress.Parse("10.183.94.41"))
                && p.Port.Equals(6881)
                && Enumerable.SequenceEqual(p.PeerId, new byte[20])));
        }
    }
}
