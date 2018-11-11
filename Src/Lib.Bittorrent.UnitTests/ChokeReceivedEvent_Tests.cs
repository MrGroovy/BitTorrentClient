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
    public class ChokeReceivedEvent_Tests
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
        public void WhenChokeReceivedEventIsExecuted_ThenItIsRegisteredInTheState()
        {
            // Arrange
            var chokeReveived = new ChokeReceivedEvent(
                IPAddress.Loopback,
                6881,
                new ChokeMessage(),
                state.Object,
                Mock.Of<ILogger<ChokeReceivedEvent>>());

            // Act
            chokeReveived.Execute(loop.Object);

            // Assert
            state.Verify(m => m.SetIsHeChoking(IPAddress.Loopback, 6881, true));
        }
    }
}
