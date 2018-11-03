using Lib.Bittorrent.Messages;
using Lib.Bittorrent.Messages.Events;
using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;

namespace Lib.Bittorrent.UnitTests
{
    [TestClass]
    public class BitfieldReceivedEvent_Tests
    {
        private Mock<ITorrentState> state;
        private Mock<IMessageLoop> loop;

        [TestInitialize]
        public void SetUp()
        {
            state = new Mock<ITorrentState>();
            state
                .Setup(m => m.RunInLock(It.IsAny<Action>()))
                .Callback<Action>(a => a.Invoke());

            loop = new Mock<IMessageLoop>();
        }

        [TestMethod]
        public void WhenBitfieldReceivedEventHasPaddingBitsSet_ThenReceiveErrorEventIsPostedToTheLoop()
        {
            // Arrange
            int numberOfPieces = 3;
            byte[] bitfieldBits = new byte[] { 0b00001111 };

            state
                .SetupGet(m => m.NumberOfPieces)
                .Returns(numberOfPieces);

            var bitfieldReceived = new BitfieldReceivedEvent(
                IPAddress.Loopback,
                6881,
                new BitfieldMessage(bitfieldBits),
                state.Object,
                Mock.Of<ILogger<BitfieldReceivedEvent>>());

            // Act
            bitfieldReceived.Execute(loop.Object);

            // Assert
            loop.Verify(m => m.PostReceiveErrorEvent(
                IPAddress.Loopback,
                6881));
        }

        [TestMethod]
        public void WhenBitfieldReceivedEventIsValid_ThenTheCorrectPiecesAreMarkedAsAvailable()
        {
            // Arrange
            int numberOfPieces = 5;
            byte[] bitfieldBits = new byte[] { 0b11011000 };

            state
                .SetupGet(m => m.NumberOfPieces)
                .Returns(numberOfPieces);

            var bitfieldReceived = new BitfieldReceivedEvent(
                IPAddress.Loopback,
                6881,
                new BitfieldMessage(bitfieldBits),
                state.Object,
                Mock.Of<ILogger<BitfieldReceivedEvent>>());

            // Act
            bitfieldReceived.Execute(loop.Object);

            // Assert
            state.Verify(m => m.MarkPieceAsAvailable(IPAddress.Loopback, 6881, 0), Times.Once);
            state.Verify(m => m.MarkPieceAsAvailable(IPAddress.Loopback, 6881, 1), Times.Once);
            state.Verify(m => m.MarkPieceAsAvailable(IPAddress.Loopback, 6881, 2), Times.Never);
            state.Verify(m => m.MarkPieceAsAvailable(IPAddress.Loopback, 6881, 3), Times.Once);
            state.Verify(m => m.MarkPieceAsAvailable(IPAddress.Loopback, 6881, 4), Times.Once);
            state.Verify(m => m.MarkPieceAsAvailable(IPAddress.Loopback, 6881, 5), Times.Never);
            state.Verify(m => m.MarkPieceAsAvailable(IPAddress.Loopback, 6881, 6), Times.Never);
            state.Verify(m => m.MarkPieceAsAvailable(IPAddress.Loopback, 6881, 7), Times.Never);
        }
    }
}
