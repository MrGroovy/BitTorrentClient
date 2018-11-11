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
    public class NotInterestedReceivedEvent_Tests
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
        public void WhenNotInterestedReceivedEventIsExecuted_ThenItIsRegisteredInTheState()
        {
            // Arrange
            var notInterested = new NotInterestedReceivedEvent(
                IPAddress.Loopback,
                6881,
                new NotInterestedMessage(),
                state.Object,
                Mock.Of<ILogger<NotInterestedReceivedEvent>>());

            // Act
            notInterested.Execute(loop.Object);

            // Assert
            state.Verify(m => m.SetIsHeInterested(IPAddress.Loopback, 6881, false));
        }
    }
}
