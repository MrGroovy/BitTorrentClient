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
        public List<byte[]> Pieces { get; }
        // public bool Private { get; set; }
        public List<MetaInfoFile> Files { get; }

        // Calculated
        public byte[] InfoHash { get; set; }
        public int TotalLength { get; set; }
        public int NumPieces { get { return Pieces.Count; } }

        public MetaInfo()
        {
            Pieces = new List<byte[]>();
            Files = new List<MetaInfoFile>();
        }

        public void AddPiece(byte[] piece)
        {
            if (!(piece.Length == 20)) throw new ArgumentException("Piece (hash) should be 20 bytes long.");

            Pieces.Add(piece);
        }

        public void AddFile(List<string> path, long length)
        {
            Files.Add(new MetaInfoFile(path, length));
        }
    }
}
