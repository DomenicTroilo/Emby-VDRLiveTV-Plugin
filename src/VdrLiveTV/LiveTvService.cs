using ServiceStack;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrLiveTV.Configuration;
using VdrLiveTV.vdr;
using System.IO;

namespace VdrLiveTV
{
    class LiveTvService : ILiveTvService
    {
//        private readonly ILogger _logger;
        public readonly ILogger _logger;
        private readonly PluginConfiguration _config;

        public LiveTvService(ILogger logger)
        {
            _logger = logger;

            _config = Plugin.Instance.Configuration;

            _logger.Info("[VDR LiveTV] Service created.");
        }

        #region Properties

        public string Name
        {
            get { return Plugin.Instance.Name; }
        }

        public string HomePageUrl
        {
            get { return "http://www.tvdr.de/"; }
        }

        #endregion

        #region Events

        public event EventHandler<RecordingStatusChangedEventArgs> RecordingStatusChanged;
        
        public event EventHandler DataSourceChanged;

        #endregion

        #region Status

        public async Task<LiveTvServiceStatusInfo> GetStatusInfoAsync(System.Threading.CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] GetStatusInfoAsync ...");

            var tunerInfos = new List<LiveTvTunerInfo>();

            return new LiveTvServiceStatusInfo
            {
                Version = Plugin.VERSION,
                Tuners = tunerInfos,
                Status = LiveTvServiceStatus.Ok,
            };
        }

        #endregion

        #region Channels

        public async Task<IEnumerable<ChannelInfo>> GetChannelsAsync(System.Threading.CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] GetChannelsAsync ...");

            var channels = await VdrApiClient.GetChannelsAsync(_logger);

            return channels;
        }

        public Task<List<MediaBrowser.Model.Dto.MediaSourceInfo>> GetChannelStreamMediaSources(string channelId, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Error("[VDR LiveTV] GetChannelStreamMediaSources not implemented.");
            throw new NotImplementedException();
        }

        public async Task<MediaBrowser.Model.Dto.MediaSourceInfo> GetChannelStream(string channelId, string streamId, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] GetChannelStream {0} - {1} ...", channelId, streamId);

            string url = string.Format("http://{0}:{1}/TS/{2}", _config.VDR_ServerName, _config.VDR_HTTP_Port, channelId);

            _logger.Info("[VDR LiveTV] StreamUrl: {0}", url);

            var mediaSourceInfo = BuildMediaSourceInfo(channelId, url);

            return mediaSourceInfo;
        }

        public async Task<MediaBrowser.Controller.Drawing.ImageStream> GetChannelImageAsync(string channelId, System.Threading.CancellationToken cancellationToken)
        {
            // Leave as is. This is handled by supplying image url to ChannelInfo
            throw new NotImplementedException();
        }

        public async Task CloseLiveStream(string id, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Debug("[VDR LiveTV] CloseLiveStream ...");
        }

        #endregion

        #region EPG

        public async Task<IEnumerable<ProgramInfo>> GetProgramsAsync(string channelId, DateTime startDateUtc, DateTime endDateUtc, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] GetProgramsAsync | {0} | {1} - {2} ...", channelId, startDateUtc.ToLocalTime(), endDateUtc.ToLocalTime());
	   try
           {
            var events = await VdrApiClient.GetEventAsync(channelId, startDateUtc.ToLocalTime(), endDateUtc.ToLocalTime(), _logger);
            return events;
	   }
	   catch (Exception ex)
	   {
              _logger.Info("[VDR LiveTV] GetProgramsAsync - no events | {0} | {1} - {2} | {3}...", channelId, startDateUtc.ToLocalTime(), endDateUtc.ToLocalTime(),ex.Message);
	       return null;
	   }
        }

        public async Task<MediaBrowser.Controller.Drawing.ImageStream> GetProgramImageAsync(string programId, string channelId, System.Threading.CancellationToken cancellationToken)
	{
            // Leave as is. This is handled by supplying image url to ChannelInfo
	    throw new NotImplementedException();
        }

        #endregion

        #region Recordings

        public async Task<IEnumerable<RecordingInfo>> GetRecordingsAsync(System.Threading.CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] GetRecordingsAsync ...");

            var recordings = await VdrApiClient.GetRecordingsAsync();

            return recordings;
        }

        public Task RecordLiveStream(string id, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Error("[VDR LiveTV] RecordLiveStream not implemented.");
            throw new NotImplementedException();
        }

        public async Task DeleteRecordingAsync(string recordingId, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] DeleteRecordingAsync ...");

            await VdrApiClient.DeleteRecording(recordingId);
        }

        public Task<MediaBrowser.Controller.Drawing.ImageStream> GetRecordingImageAsync(string recordingId, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Error("[VDR LiveTV] GetRecordingImageAsync not implemented.");
            throw new NotImplementedException();
        }

        public async Task<MediaBrowser.Model.Dto.MediaSourceInfo> GetRecordingStream(string recordingId, string streamId, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] GetRecordingStream ...");

            string url = string.Format("http://{0}:{1}/{2}.rec.ts", _config.VDR_ServerName, _config.VDR_HTTP_Port, int.Parse(recordingId) + 1);

            _logger.Info("[VDR LiveTV] StreamUrl: {0}", url);

            var mediaSourceInfo = BuildMediaSourceInfo(recordingId, url);

            return mediaSourceInfo;
        }

        public Task<List<MediaBrowser.Model.Dto.MediaSourceInfo>> GetRecordingStreamMediaSources(string recordingId, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Error("[VDR LiveTV] GetRecordingStreamMediaSources not implemented.");
            throw new NotImplementedException();
        }

        #endregion

        #region Timers

        public async Task<IEnumerable<TimerInfo>> GetTimersAsync(System.Threading.CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] GetTimersAsync ...");

            var timers = await VdrApiClient.GetTimersAsync();

            return timers;
        }

        public async Task CreateTimerAsync(TimerInfo info, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] CreateTimerAsync ...");

            await VdrApiClient.CreateTimer(info);
        }

        public async Task UpdateTimerAsync(TimerInfo info, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] UpdateTimerAsync ...");
            _logger.Info(info.ToJson());

            await VdrApiClient.UpdateTimer(info);
        }

        public async Task CancelTimerAsync(string timerId, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] CancelTimerAsync ...");

            await VdrApiClient.CancelTimer(timerId);
        }

        public async Task<SeriesTimerInfo> GetNewTimerDefaultsAsync(System.Threading.CancellationToken cancellationToken, ProgramInfo program = null)
        {
            _logger.Info("[VDR LiveTV] GetNewTimerDefaultsAsync ...");

            return await Task.Factory.StartNew<SeriesTimerInfo>(() =>
            {
                return new SeriesTimerInfo
                {
                    PostPaddingSeconds = 0,
                    PrePaddingSeconds = 0,
                    RecordAnyChannel = true,
                    RecordAnyTime = true,
                    RecordNewOnly = false
                };
            });
        }

        #endregion

        #region Series timers

        public Task<IEnumerable<SeriesTimerInfo>> GetSeriesTimersAsync(System.Threading.CancellationToken cancellationToken)
        {
            _logger.Error("[VDR LiveTV] GetSeriesTimersAsync not implemented.");
            throw new NotImplementedException();
        }

        public Task CreateSeriesTimerAsync(SeriesTimerInfo info, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Error("[VDR LiveTV] CreateSeriesTimerAsync not implemented.");
            throw new NotImplementedException();
        }

        public Task UpdateSeriesTimerAsync(SeriesTimerInfo info, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Error("[VDR LiveTV] UpdateSeriesTimerAsync not implemented.");
            throw new NotImplementedException();
        }

        public Task CancelSeriesTimerAsync(string timerId, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Error("[VDR LiveTV] CancelSeriesTimerAsync not implemented.");
            throw new NotImplementedException();
        }

        #endregion
        
        #region Misc

        public Task ResetTuner(string id, System.Threading.CancellationToken cancellationToken)
        {
            _logger.Error("[VDR LiveTV] ResetTuner not implemented.");
            throw new NotImplementedException();
        }

        #endregion

        private IVdrApiClient VdrApiClient
        {
            get { return new VdrRestfulApiClient(_config.VDR_ServerName, _config.VDR_RestfulApi_Port); }
        }

        private static MediaBrowser.Model.Dto.MediaSourceInfo BuildMediaSourceInfo(string id, string url)
        {
            var mediaSourceInfo = new MediaBrowser.Model.Dto.MediaSourceInfo()
            {
                Id = id,
                Path = url,
                Protocol = MediaProtocol.Http,
	        SupportsProbing = false,
                MediaStreams = new List<MediaStream>
                        {
                            new MediaStream
                            {
                                Type = MediaStreamType.Video,
                                // Set the index to -1 because we don't know the exact index of the video stream within the container
                                Index = -1,
                                // Set to true if unknown to enable deinterlacing
                                IsInterlaced = true
                            },
                            new MediaStream
                            {
                                Type = MediaStreamType.Audio,
                                // Set the index to -1 because we don't know the exact index of the audio stream within the container
                                Index = -1
                            }
                        }
            };
            return mediaSourceInfo;
        }

    }
}
