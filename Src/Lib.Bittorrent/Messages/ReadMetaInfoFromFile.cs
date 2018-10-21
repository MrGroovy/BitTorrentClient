using Lib.Bittorrent.MetainfoDecoding;
using Lib.Bittorrent.StateManagement;
using System.IO;
using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    /// <remarks>
    /// - ToDo: Handle file not exist?
    /// - ToDo: Handle file read failes?
    /// - ToDo: Handle deserializarion failes?
    /// </remarks>
    public class ReadMetaInfoFromFile : Message
    {
        private string filepath;
        private TorrentState state;

        public ReadMetaInfoFromFile(string filepath, TorrentState state)
        {
            this.filepath = filepath;
            this.state = state;
        }

        public override Task Execute(MessageLoop loop)
        {
            state.MetaInfo = ReadFromFile();
            loop.PostCallTrackerMessage();
            return Task.CompletedTask;
        }

        public MetaInfo ReadFromFile()
        {
            using (Stream stream = new MemoryStream(File.ReadAllBytes(filepath)))
            {
                var deserializer = new MetaInfoSerializer();
                return deserializer.Deserialize(stream);
            }
        }
    }
}
