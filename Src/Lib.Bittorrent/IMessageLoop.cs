﻿using Lib.Bittorrent.Swarm;
using System.Net;

namespace Lib.Bittorrent
{
    public interface IMessageLoop
    {
        void PostReadMetaInfoFromFileMessage(string filePath);
        void PostCallTrackerMessage();
        void PostConnectToPeerMessage(IPAddress ip, int port, byte[] peerId);
        void PostPeerConnectedMessage(IPAddress ip, int port);
        void PostHandshakeReceivedMessage(IPAddress ip, int port, HandshakeMessage handshake);
        void PostKeepAliveReceivedMessage(IPAddress ip, int port);
        void PostDecideWhatToDoMessage();
    }
}
