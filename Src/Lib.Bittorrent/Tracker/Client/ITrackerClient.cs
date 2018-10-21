using System.Threading.Tasks;
using Lib.Bittorrent.Tracker.Dto;

namespace Lib.Bittorrent.Tracker.Client
{
    public interface ITrackerClient
    {
        Task<TrackerResponseDto> CallTracker(string uri, TrackerRequestDto request);
    }
}