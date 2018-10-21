using Lib.Bittorrent.StateManagement;
using Lib.Bittorrent.Swarm;
using Lib.Bittorrent.Tracker.Client;
using Microsoft.Extensions.Logging;
using System;
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
            RunBittorrentClient();
            //ExperimentWithActionBlock();
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
            var swarm = new PeerSwarm(msgLoop, logFactory);

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
