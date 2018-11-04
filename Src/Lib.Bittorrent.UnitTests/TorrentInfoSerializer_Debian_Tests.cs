using Lib.Bittorrent.MetainfoDecoding;
using Lib.Bittorrent.StateManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    [DeploymentItem(@"TorrentFiles")]
    public class TorrentInfoSerializer_Debian_Tests
    {
        private Stream bencoding;
        private MetaInfoSerializer serializer;

        [TestInitialize]
        public void SetUp()
        {
            bencoding = new MemoryStream(File.ReadAllBytes(@"TorrentFiles\debian-9.4.0-amd64-netinst.iso.torrent"));
            serializer = new MetaInfoSerializer();
        }

        [TestCleanup]
        public void TearDown()
        {
            bencoding.Dispose();
        }

        [TestMethod]
        public void WhenTorrentFileHasAnnounce_ThenItIsDeserialized()
        {
            // Act
            MetaInfo torrentInfo =  serializer.Deserialize(bencoding);

            // Assert
            Assert.AreEqual("http://bttracker.debian.org:6969/announce", torrentInfo.Announce);
        }

        [TestMethod]
        public void WhenTorrentFileHasComment_ThenItIsDeserialized()
        {
            // Act
            MetaInfo torrentInfo = serializer.Deserialize(bencoding);

            // Assert
            Assert.AreEqual("\"Debian CD from cdimage.debian.org\"", torrentInfo.Comment);
        }

        [TestMethod]
        public void WhenTorrentFileHasOneFile_ThenItIsDeserialized()
        {
            // Act
            MetaInfo torrentInfo = serializer.Deserialize(bencoding);

            // Assert
            var file1 = torrentInfo.Files[0];

            Assert.AreEqual(1, torrentInfo.Files.Count);
            Assert.IsTrue(file1.Length == 305135616 && file1.Path[0] == "debian-9.4.0-amd64-netinst.iso");
        }

        [TestMethod]
        public void WhenTorrentHasPieceLength_ThenItIsDeserialized()
        {
            // Act
            MetaInfo torrentInfo = serializer.Deserialize(bencoding);

            // Assert
            Assert.AreEqual(262144, torrentInfo.PieceLength);
        }

        [TestMethod]
        public void WhenTorrentFileHasPiecesStringOfLength23280_ThenTheyAreDeserializedIn1164Pieces()
        {
            // Act
            MetaInfo torrentInfo = serializer.Deserialize(bencoding);

            // Assert
            Assert.AreEqual(1164, torrentInfo.PieceHashes.Count);
        }

        [TestMethod]
        public void WhenTorrentFileIsDeserialized_ThenTheInfoHashShouldBeCorrect()
        {
            // Act
            MetaInfo torrentInfo = serializer.Deserialize(bencoding);

            // Assert
            byte[] expectedInfoHash =
            {
                116, 49, 169, 105, 179, 71, 225, 75, 186, 100,
                27, 53, 23, 192, 36, 247, 180, 13, 251, 127
            };

            CollectionAssert.AreEqual(expectedInfoHash, torrentInfo.InfoHash);
        }
    }
}
