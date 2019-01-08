//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Abstractions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace Lib.Bittorrent.UnitTests.Tests
//{
//    [TestClass]
//    public class TorrentController_StartFromFile_Tests
//    {
//        [TestInitialize]
//        public void SetUp()
//        {
//        }

//        [TestMethod]
//        public void When_TorrentFileHasAnnounce_ThenItIsReadInMetaInfo()
//        {
//            // Act
//            controller.StartFromFile(@"TestInput\TorrentFiles\Debian.torrent");

//            // Assert
//            Assert.AreEqual("http://bttracker.debian.org:6969/announce", controller.Info.Announce);
//        }

//        [TestMethod]
//        public void When_TorrentFileHasComment_ThenItIsReadInMetaInfo()
//        {
//            // Act
//            controller.StartFromFile(@"TestInput\TorrentFiles\Debian.torrent");

//            // Assert
//            Assert.AreEqual("\"Debian CD from cdimage.debian.org\"", controller.Info.Comment);
//        }

//        [TestMethod]
//        public void When_TorrentFileHasPieceLength_ThenItIsReadInMetaInfo()
//        {
//            // Act
//            controller.StartFromFile(@"TestInput\TorrentFiles\Debian.torrent");

//            // Assert
//            Assert.AreEqual(262144, controller.Info.PieceLength);
//        }

//        [TestMethod]
//        public void When_TorrentFileHasPieces_ThenItIsReadInMetaInfo()
//        {
//            // Act
//            controller.StartFromFile(@"TestInput\TorrentFiles\Debian.torrent");

//            // Assert
//            Assert.AreEqual(1164, controller.Info.PieceHashes.Count);
//        }

//        [TestMethod]
//        public void When_TorrentFileIsRead_ThenInfoHashShouldBeCorrectlyCalculated()
//        {
//            // Act
//            controller.StartFromFile(@"TestInput\TorrentFiles\Debian.torrent");

//            // Assert
//            byte[] expectedInfoHash =
//            {
//                116, 49, 169, 105, 179,
//                71, 225, 75, 186, 100,
//                27, 53, 23, 192, 36,
//                247, 180, 13, 251, 127
//            };

//            CollectionAssert.AreEqual(expectedInfoHash, controller.Info.InfoHash);
//        }

//        [TestMethod]
//        public void When_TorrentFileHasOneFile_ThenFileIsReadInMetaInfo()
//        {
//            // Act
//            controller.StartFromFile(@"TestInput\TorrentFiles\HasOneFile.torrent");

//            // Assert
//            var file1 = controller.Info.Files[0];

//            Assert.AreEqual(1, controller.Info.Files.Count);
//            Assert.IsTrue(file1.Length == 305135616 && file1.Path[0] == "debian-9.4.0-amd64-netinst.iso");
//        }

//        [TestMethod]
//        public void When_TorrentFileHasMultipleFiles_ThenItIsReadInMetaInfo()
//        {
//            // Act
//            controller.StartFromFile(@"TestInput\TorrentFiles\HasMultipleFiles.torrent");

//            // Assert
//            var file1 = controller.Info.Files[0];
//            var file2 = controller.Info.Files[1];
//            var file3 = controller.Info.Files[2];
//            var file4 = controller.Info.Files[3];
//            var file5 = controller.Info.Files[4];

//            Assert.AreEqual(5, controller.Info.Files.Count);
//            Assert.IsTrue(file1.Length == 129919357 && file1.Path[0] == "ElectronMicroscopy_Hippocampus" && file1.Path[1] == "testing.tif");
//            Assert.IsTrue(file2.Length == 129919357 && file2.Path[0] == "ElectronMicroscopy_Hippocampus" && file2.Path[1] == "testing_groundtruth.tif");
//            Assert.IsTrue(file3.Length == 129919357 && file3.Path[0] == "ElectronMicroscopy_Hippocampus" && file3.Path[1] == "training.tif");
//            Assert.IsTrue(file4.Length == 129919357 && file4.Path[0] == "ElectronMicroscopy_Hippocampus" && file4.Path[1] == "training_groundtruth.tif");
//            Assert.IsTrue(file5.Length == 3353674357 && file5.Path[0] == "ElectronMicroscopy_Hippocampus" && file5.Path[1] == "volumedata.tif");
//        }
//    }
//}
