using Lib.Bittorrent.Tracker.Dto;
using Lib.Bencoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Lib.Bittorrent.Tracker.Client
{
    public class TrackerResponseSerializer
    {
        public TrackerResponseDto Deserialize(Stream bencoding)
        {
            var bencodingParser = new BenValueParser(bencoding);
            BenDictionary responseDict = (BenDictionary)bencodingParser.Parse();

            TrackerResponseDto response = new TrackerResponseDto();
            response.Interval = ReadInterval(responseDict, response);
            response.AddPeers(ReadPeers(responseDict));
            return response;
        }

        private int ReadInterval(BenDictionary responseDict, TrackerResponseDto response)
        {
            return responseDict.HasInt("interval")
                ? (int)responseDict.GetInt("interval").Value
                : response.Interval;
        }

        private List<(IPAddress ip, int port, byte[] peerId)> ReadPeers(BenDictionary responseDict)
        {
            if (responseDict.HasList("peers"))
            {
                return ReadPeersFromListOfDicts(responseDict);
            }
            else if (responseDict.HasString("peers"))
            {
                return ReadPeersFromString(responseDict);
            }
            else
            {
                throw new InvalidOperationException("Invalid tracker response, peers are missing.");
            }
        }

        private List<(IPAddress ip, int port, byte[] peerId)> ReadPeersFromListOfDicts(BenDictionary responseDict)
        {
            return responseDict
                .GetListOfDicts("peers")
                .Select(d => ReadPeerFromDict(d))
                .ToList();
        }

        private (IPAddress ip, int port, byte[] peerId) ReadPeerFromDict(BenDictionary peerDict)
        {
            return (
                IPAddress.Parse(peerDict.GetString("ip")),
                (int)peerDict.GetInt("port").Value,
                peerDict.GetStringBytes("peer id"));
        }

        private List<(IPAddress ip, int port, byte[] peerId)> ReadPeersFromString(BenDictionary responseDict)
        {
            byte[] peersBytes = responseDict.GetStringBytes("peers");
            return Enumerable.Range(0, peersBytes.Length / 6)
                .Select(i => peersBytes.Skip(i * 6).Take(6).ToArray())
                .Select(p => ReadPeerFromSixBytes(p))
                .ToList();
        }

        private (IPAddress ip, int port, byte[] peerId) ReadPeerFromSixBytes(byte[] peerBytes)
        {
            byte[] ipBytes = peerBytes.Take(4).ToArray();
            byte[] portBytes = peerBytes.Skip(4).Take(2).ToArray();

            return (
                ip: new IPAddress(ipBytes),
                port: (portBytes[0] << 8) | (portBytes[1]),
                peerId: new byte[20]);
        }
    }
}
