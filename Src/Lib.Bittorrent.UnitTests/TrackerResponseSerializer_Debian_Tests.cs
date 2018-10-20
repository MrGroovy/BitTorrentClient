using Lib.Bittorrent.Tracker.Client;
using Lib.Bittorrent.Tracker.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    [DeploymentItem(@"TrackerResponses")]
    public class TrackerResponseSerializer_Debian_Tests
    {
        private Stream bencoding;
        private TrackerResponseSerializer serializer;

        [TestInitialize]
        public void SetUp()
        {
            bencoding = new MemoryStream(File.ReadAllBytes(@"TrackerResponses\debian-9.4.0-amd64-netinst.iso.trackerResponse"));
            serializer = new TrackerResponseSerializer();
        }

        [TestCleanup]
        public void TearDown()
        {
            bencoding.Dispose();
        }

        [TestMethod]
        public void WhenTrackerResponseHasInterval_ThenItIsDeserialized()
        {
            // Act
            TrackerResponseDto response = serializer.Deserialize(bencoding);

            // Assert
            Assert.AreEqual(900, response.Interval);
        }

        [TestMethod]
        public void WhenTrackerResponseContainsPeersAsBinaryString_ThenTheyAreDeserialized()
        {
            // Act
            TrackerResponseDto response = serializer.Deserialize(bencoding);

            // Assert
            Assert.IsTrue(response.Peers.Count > 0);

            //var peer1 = response.Peers[0];
            //var peer2 = response.Peers[1];
            //var peer3 = response.Peers[2];

            //Assert.AreEqual(3, response.Peers.Count);
            //Assert.AreEqual(new IPAddress(new byte[] { 209, 181, 242, 180 }), peer1.Ip);
            //Assert.AreEqual(16881, peer1.Port);

            //Assert.AreEqual(new IPAddress(new byte[] { 70, 57, 182, 176 }), peer2.Ip);
            //Assert.AreEqual(33761, peer2.Port);

            //Assert.AreEqual(new IPAddress(new byte[] { 185, 149, 90, 114 }), peer3.Ip);            
            //Assert.AreEqual(51033, peer3.Port);
        }
    }
}
