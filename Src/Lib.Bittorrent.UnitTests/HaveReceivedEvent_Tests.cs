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
    public class HaveReceivedEvent_Tests
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
        public void WhenHaveReceivedEventIsValid_ThenTheCorrectPieceIsMarkedAsAvailable()
        {
            // Arrange
            var haveReceived = new HaveReceivedEvent(
                IPAddress.Loopback,
                6881,
                new HaveMessage(44),
                state.Object,
                Mock.Of<ILogger<HaveReceivedEvent>>());

            // Act
            haveReceived.Execute(loop.Object);

            // Assert
            state.Verify(m => m.MarkPieceAsAvailable(IPAddress.Loopback, 6881, 44), Times.Once);
        }

        [TestMethod]
        public void WhenHaveReceivedEventRefersToInvalidPieceIndex_ThenReceiveErrorEventIsPostedToTheLoop()
        {
            // Arrange
            state
                .Setup(m => m.MarkPieceAsAvailable(IPAddress.Loopback, 6881, 44))
                .Throws(new ArgumentOutOfRangeException());

            var haveReceived = new HaveReceivedEvent(
                IPAddress.Loopback,
                6881,
                new HaveMessage(44),
                state.Object,
                Mock.Of<ILogger<HaveReceivedEvent>>());

            // Act
            haveReceived.Execute(loop.Object);

            // Assert
            loop.Verify(m => m.PostReceiveErrorEvent(
                IPAddress.Loopback,
                6881));
        }
    }
}
