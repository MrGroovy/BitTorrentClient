using Lib.Bittorrent.Tracker.Dto;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Lib.Bittorrent.Tracker.Client
{
    public class TrackerClient : ITrackerClient
    {
        private HttpClient httpClient;
        private TrackerResponseSerializer ser;

        public TrackerClient(HttpMessageHandler handler = null)
        {
            httpClient = new HttpClient(handler ?? new HttpClientHandler());
            ser = new TrackerResponseSerializer();
        }

        public async Task<TrackerResponseDto> CallTracker(string uri, TrackerRequestDto request)
        {
            HttpResponseMessage httpRes = await httpClient.GetAsync(TrackerRequestToUri(uri, request));
            return HttpResponseToTrackerResponse(httpRes);
        }

        private string TrackerRequestToUri(string uri, TrackerRequestDto req)
        {
            var query = new Dictionary<string, string>();
            query.Add("info_hash", HttpUtility.UrlEncode(req.InfoHash));
            query.Add("peer_id", HttpUtility.UrlEncode(req.PeerId));
            query.Add("port", req.Port.ToString());
            query.Add("uploaded", req.Uploaded.ToString());
            query.Add("downloaded", req.Downloaded.ToString());
            query.Add("left", req.Left.ToString());
            query.Add("compact", "1");
            query.Add("event", "started");

            string queryString = CreateQueryString(query);
            return $"{uri}?{queryString}";
        }

        private string CreateQueryString(Dictionary<string, string> query)
        {
            string queryString = string.Join(
                '&',
                query.Select(p => $"{p.Key}={p.Value}"));
            return queryString;
        }

        private TrackerResponseDto HttpResponseToTrackerResponse(HttpResponseMessage res)
        {
            using (Stream stream = res.Content.ReadAsStreamAsync().Result)
            {
                return ser.Deserialize(stream);
            }
        }
    }
}
