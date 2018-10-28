using System.Threading.Tasks;

namespace Lib.Bittorrent.Messages
{
    public abstract class Message
    {
        public abstract Task Execute(IMessageLoop loop);
    }
}
