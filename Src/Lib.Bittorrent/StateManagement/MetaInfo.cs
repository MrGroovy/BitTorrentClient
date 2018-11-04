using System;
using System.Collections.Generic;

namespace Lib.Bittorrent.StateManagement
{
    public class MetaInfo
    {
        public string Announce { get; set; }
        //public List<string> AnnounceList { get; set; }
        //public DateTime? CreationTime { get; set; }
        public string Comment { get; set; }

        // Info Dictionary
        public long PieceLength { get; set; }
        public List<byte[]> PieceHashes { get; }
        // public bool Private { get; set; }
        public List<MetaInfoFile> Files { get; }

        // Calculated
        public byte[] InfoHash { get; set; }
        public int TotalLength { get; set; }
        public int NumPieces { get { return PieceHashes.Count; } }

        public MetaInfo()
        {
            PieceHashes = new List<byte[]>();
            Files = new List<MetaInfoFile>();
        }

        public void AddPiece(byte[] pieceHash)
        {
            if (pieceHash.Length != 20) throw new ArgumentException("Piecehash should be 20 bytes long.");

            PieceHashes.Add(pieceHash);
        }

        public void AddFile(List<string> path, long length)
        {
            Files.Add(new MetaInfoFile(path, length));
        }
    }
}
