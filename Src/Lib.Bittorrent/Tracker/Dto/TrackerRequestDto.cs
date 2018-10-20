namespace Lib.Bittorrent.Tracker.Dto
{
    public class TrackerRequestDto
    {
        public byte[] InfoHash { get; set; }
        public byte[] PeerId { get; set; }
        public int Port { get; set; }
        public int Uploaded { get; set; }
        public int Downloaded { get; set; }
        public int Left { get; set; }
    }
}
