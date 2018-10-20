using Lib.Bittorrent.MetainfoDecoding;
using Lib.Bittorrent.StateManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Lib.Bittorrent.UnitTests
{
    /// <summary>
    /// http://academictorrents.com/details/3ada3ae6ec71097e63d897cf878051bba3eaba25
    /// </summary>
    [TestClass]
    [DeploymentItem(@"TorrentFiles")]
    public class TorrentInfoSerializer_ElectronMicroscopyDataset_Tests
    {
        private Stream bencoding;
        private MetaInfoSerializer serializer;

        [TestInitialize]
        public void SetUp()
        {
            bencoding = new MemoryStream(File.ReadAllBytes(@"TorrentFiles\Electron Microscopy (CA1 hippocampus) Dataset.torrent"));
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
            MetaInfo torrentInfo = serializer.Deserialize(bencoding);

            // Assert
            Assert.AreEqual("http://academictorrents.com/announce.php", torrentInfo.Announce);
        }

        [TestMethod]
        public void WhenTorrentFileHasNoComment_ThenCommentIsNull()
        {
            // Act
            MetaInfo torrentInfo = serializer.Deserialize(bencoding);

            // Assert
            Assert.IsNull(torrentInfo.Comment);
        }

        [TestMethod]
        public void WhenTorrentFileHasMultipleFiles_ThenTheyAreDeserialized()
        {
            // Act
            MetaInfo torrentInfo = serializer.Deserialize(bencoding);

            // Assert
            var file1 = torrentInfo.Files[0];
            var file2 = torrentInfo.Files[1];
            var file3 = torrentInfo.Files[2];
            var file4 = torrentInfo.Files[3];
            var file5 = torrentInfo.Files[4];

            Assert.AreEqual(5, torrentInfo.Files.Count);
            Assert.IsTrue(file1.Length == 129919357 && file1.Path[0] == "ElectronMicroscopy_Hippocampus" && file1.Path[1] == "testing.tif");
            Assert.IsTrue(file2.Length == 129919357 && file2.Path[0] == "ElectronMicroscopy_Hippocampus" && file2.Path[1] == "testing_groundtruth.tif");
            Assert.IsTrue(file3.Length == 129919357 && file3.Path[0] == "ElectronMicroscopy_Hippocampus" && file3.Path[1] == "training.tif");
            Assert.IsTrue(file4.Length == 129919357 && file4.Path[0] == "ElectronMicroscopy_Hippocampus" && file4.Path[1] == "training_groundtruth.tif");
            Assert.IsTrue(file5.Length == 3353674357 && file5.Path[0] == "ElectronMicroscopy_Hippocampus" && file5.Path[1] == "volumedata.tif");
        }

        [TestMethod]
        public void WhenTorrentHasPieceLength_ThenItIsDeserialized()
        {
            // Act
            MetaInfo torrentInfo = serializer.Deserialize(bencoding);

            // Assert
            Assert.AreEqual(1048576, torrentInfo.PieceLength);
        }

        [TestMethod]
        public void WhenTorrentFileHasPiecesStringOfLength73880_ThenTheyAreDeserializedIn3694Pieces()
        {
            // Act
            MetaInfo torrentInfo = serializer.Deserialize(bencoding);

            // Assert
            Assert.AreEqual(3694, torrentInfo.Pieces.Count);
        }

        [TestMethod]
        public void WhenTorrentFileIsDeserialized_ThenTheInfoHashShouldBeCorrect()
        {
            // Act
            MetaInfo torrentInfo = serializer.Deserialize(bencoding);

            // Assert
            byte[] expectedInfoHash =
            {
                58, 218, 58, 230, 236, 113, 9, 126, 99, 216,
                151, 207, 135, 128, 81, 187, 163, 234, 186, 37,                
            };

            CollectionAssert.AreEqual(expectedInfoHash, torrentInfo.InfoHash);
        }
    }
}
