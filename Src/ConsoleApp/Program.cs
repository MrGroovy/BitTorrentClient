using Lib.Bittorrent;
using Lib.Bittorrent.MetainfoDecoding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RunBittorrentClient();

            Console.ReadLine();
        }

        public static void RunBittorrentClient()
        {
            var services = new ServiceCollection();
            services.AddLogging(options =>
            {
                options.AddConsole();
                options.SetMinimumLevel(LogLevel.Debug);
            });
            services.AddTransient<Torrent>();
            var serviceProvider = services.BuildServiceProvider();

            string torrentFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                "..", "..", "..", "..", "..", "TestTorrents", "debian-9.4.0-amd64-netinst.iso.torrent");
            MetaInfo metaInfo = null;
            using (Stream stream = File.OpenRead(torrentFilePath))
            {
                var serializer = new MetaInfoSerializer();
                metaInfo = serializer.Deserialize(stream);
            }

            var logFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            Torrent torrent = new Torrent(metaInfo, logFactory, logFactory.CreateLogger<Torrent>());
        }
    }
}
