using System.Threading.Tasks;

namespace Lib.Bittorrent
{
    public abstract class Message
    {
        public abstract Task Execute(MessageLoop loop);
    }
}
