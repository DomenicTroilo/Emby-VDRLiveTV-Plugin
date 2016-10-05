using MediaBrowser.Model.LiveTv;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VdrLiveTV.vdr
{
    public interface IVdrApiClient
    {
        Task<List<ChannelInfo>> GetChannelsAsync(ILogger logger);
        Task<List<ProgramInfo>> GetEventAsync(string channelId, DateTime startDateUtc, DateTime endDateUtc, ILogger logger);
//        Task<byte[]> GetEventImageAsync(int evendId, int imageNumber);
        Task<List<RecordingInfo>> GetRecordingsAsync();
        Task DeleteRecording(string recordingId);
        Task<List<TimerInfo>> GetTimersAsync();
        Task CreateTimer(TimerInfo info);
        Task UpdateTimer(TimerInfo info);
        Task CancelTimer(string timerId);
    }
}
