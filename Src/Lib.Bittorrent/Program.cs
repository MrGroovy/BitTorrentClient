using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.Tracker.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Lib.Bittorrent
{
    public class Program
    {
        private static int counter;
        private static SemaphoreSlim locker;

        public static void Main(string[] args)
        {
            //ExperimentWithSocket();
            RunBittorrentClient();
            //ExperimentWithActionBlock();
            //Task.Run(ExperimentWithTcpClient);
            Console.ReadLine();
        }

        private static async Task ExperimentWithSocket()
        {
            try
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync("www.reinverberne.nl", 80);
                socket.Close();

                var arrBuffer = new byte[20];
                var memBuffer = new Memory<byte>(arrBuffer);
                int bytesRcvd = await socket.ReceiveAsync(memBuffer, SocketFlags.None);

                //Task loop = ReceiveLoopSocket(socket);
                //await Task.Delay(100);

            }
            catch (Exception ex)
            {
            }
        }

        private static async Task ReceiveLoopSocket(Socket socket)
        {
            try
            {
                

                var arrBuffer = new byte[20];
                var memBuffer = new Memory<byte>(arrBuffer);
                Console.WriteLine("ReceiveAsync");
                int bytesRcvd = await socket.ReceiveAsync(memBuffer, SocketFlags.None);
                Console.WriteLine("ReceiveAsync end");
            }
            catch (Exception ex)
            {
            }
        }

        private static async Task ExperimentWithTcpClient()
        {
            TcpClient c = new TcpClient();
            await c.ConnectAsync("www.reinverberne.nl", 80);
            Task loop = ReceiveLoop(c);
            await Task.Delay(100);
            c.Close();

            await Task.Delay(10000);
        }

        private static async Task ReceiveLoop(TcpClient c)
        {
            try
            {
                //Socket s;
                //s.ReceiveAsync(new Memory<byte>(, SocketFlags.None);

                NetworkStream ns = c.GetStream();
                await ns.WriteAsync(new byte[] { 5, 6, 7, 8, 9, 10});
                byte[] buffer = new byte[512];
                int num = await ns.ReadAsync(buffer, 0, buffer.Length);
                string resp = System.Text.Encoding.UTF8.GetString(buffer);
            }
            catch (Exception ex)
            {
            }
        }

        private static void ExperimentWithActionBlock()
        {
            locker = new SemaphoreSlim(1, 1);

            ActionBlock<int> block;
            block = new ActionBlock<int>(
                async i =>
                {
                    //if (i % 2 == 0)
                    if (i < 5000)
                    {
                        Console.WriteLine($"[{i}] Calling server...");
                        await Task.Delay(10_000);
                        Console.WriteLine($"[{i}] Done calling server");
                    }
                    else
                    {
                        await locker.WaitAsync();
                        try
                        {
                            int localCounterValue = counter;
                            Console.WriteLine($"[{i}] Starting the delay...");
                            //await Task.Delay(50);
                            counter = localCounterValue + 1;
                            Console.WriteLine($"[{i}] Done with locked section [{counter}]");
                        }
                        finally
                        {
                            locker.Release();
                        }
                    }
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = -1 });

            for (int i = 0; i < 10000; i++)
            {
                block.Post(i);
            }

            block.Complete();
            block.Completion.Wait();

            Console.ReadLine();
        }

        public static void RunBittorrentClient()
        {
            var logFactory = new LoggerFactory();
            logFactory.AddConsole(LogLevel.Trace);

            var state = new TorrentState(logFactory.CreateLogger<TorrentState>());
            var trackerClient = new TrackerClient();
            var msgFactory = new MessageFactory();
            var msgLoop = new MessageLoop(msgFactory);
            var swarm = new PeerSwarm(msgLoop, logFactory, logFactory.CreateLogger<PeerSwarm>());

            msgFactory.SetDependencies(
                state,
                trackerClient,
                swarm,
                logFactory);

            Timer timer = new Timer(
                timerState => msgLoop.PostDecideWhatToDoMessage(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1));

            string torrentFile = @"C:\Rein\Projecten\BittorrentClient\TorrentFiles\debian-9.4.0-amd64-netinst.iso.torrent";
            msgLoop.PostReadMetaInfoFromFileMessage(torrentFile);

            Console.ReadLine();
        }
    }
}
