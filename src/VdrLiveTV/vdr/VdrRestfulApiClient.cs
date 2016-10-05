using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Logging;
using MediaBrowser.Controller.LiveTv;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;

namespace VdrLiveTV.vdr
{
    public class VdrRestfulApiClient : IVdrApiClient
    {
        string baseUrl;
        JsonServiceClient client;

        public VdrRestfulApiClient(string host, int port)
        {
            baseUrl = string.Format("http://{0}:{1}", host, port);
            client = new JsonServiceClient(baseUrl);
        }

        public async Task<List<ChannelInfo>> GetChannelsAsync(ILogger logger)
        {
            List<ChannelInfo> channels = new List<ChannelInfo>();

	    logger.Info("[VDR LiveTV] GetChannelsAsync --- START --- ");
            GetChannelsResponse response = await client.GetAsync(new GetChannelsRequest());

            if (response.Channels != null)
            {
                foreach (VdrChannel channel in response.Channels)
                {
		logger.Info("[VDR LiveTV] GetChannelsAsync data | ID {0} | CHANTYPE {1} | NAME {2} | NUMBER {3} | HASIMAGE {4} | IMAGEURL {5}  ...", channel.channel_id, channel.is_radio ? ChannelType.Radio : ChannelType.TV, channel.name, channel.number.ToString(), channel.image , channel.image ? string.Format("{0}/channels/image/{1}", baseUrl, channel.channel_id) : null);
                    channels.Add(new ChannelInfo()
                    {
                        Id = channel.channel_id,
                        ChannelType = channel.is_radio ? ChannelType.Radio : ChannelType.TV,
                        Name = channel.name,
                        Number = channel.number.ToString(),
                        HasImage = channel.image,
			ImageUrl = channel.image ? string.Format("{0}/channels/image/{1}", baseUrl, channel.channel_id) : null 
                    });
                }
            }

	    logger.Info("[VDR LiveTV] GetChannelsAsync --- END --- ");
            return channels;
        }

        public async Task<List<ProgramInfo>> GetEventAsync(string channelId, DateTime startDateUtc, DateTime endDateUtc, ILogger logger)
        {
            List<ProgramInfo> events = new List<ProgramInfo>();
	  try
            {
            GetEventsResponse response = await client.GetAsync(new GetEventsRequest() {
                channel = channelId + ".json",
                timespan = (endDateUtc - startDateUtc).TotalSeconds.ToString()
            });

            if (response.Events != null)
            {
	        logger.Info("[VDR LiveTV] GetEventAsync --- START --- ");
                foreach (VdrEvent ev in response.Events)
                {
		    logger.Info("[VDR LiveTV] GetEventAsync data | CHANNELID {0} | NAME {1} | STARTDATE {2} | ENDDATE {3} | EPTITLE {4} | OVERVIEW {5} | HASIMAGE {6} | IMAGEURL {7} | Id {8} ...", ev.channel, ev.title, Utils.UnixTimeStampToDateTime(ev.start_time), Utils.UnixTimeStampToDateTime(ev.start_time + ev.duration), ev.short_text, ev.description, ev.images, (ev.images > 0) ? string.Format("{0}/events/image/{1}/1", baseUrl, ev.id) : null, ev.id.ToString());
		    events.Add(new ProgramInfo()
                    {
                        ChannelId = ev.channel,
                        Id = ev.id.ToString(),
                        Overview = ev.description,
                        StartDate = Utils.UnixTimeStampToDateTime(ev.start_time).ToUniversalTime(),
                        EndDate = Utils.UnixTimeStampToDateTime(ev.start_time + ev.duration).ToUniversalTime(),
			// Genres = null,
                	OriginalAirDate = null,
                        Name = ev.title,
			OfficialRating = null,
			CommunityRating = null,
                        EpisodeTitle = ev.short_text,
			// Audio = null,
			IsHD = null,
			IsRepeat = false,
			IsSeries = true,
			IsNews = ev.description.ToLower().Contains("news"),
			IsMovie = ev.description.ToLower().Contains("movie"),
			IsKids = ev.description.ToLower().Contains("kids"),
			IsSports = ev.description.ToLower().Contains("sports"),
// Note this is not perfect
// VDR allows multiple event images but 
// Emby only allows one
// we are providing the first image if it exists
                        HasImage = (ev.images > 0),
                        ImageUrl = (ev.images > 0) ? string.Format("{0}/events/image/{1}/1", baseUrl, ev.id) : null
                    });
		}
	        logger.Info("[VDR LiveTV] GetEventAsync --- END --- ");
            }
	    else
	    {
	      logger.Info("[VDR LiveTV] GetEventAsync No EVENTS for {0}", channelId);
	    }
            return events;
	  }
	  catch (Exception ex)
	  {
	      logger.Info("[VDR LiveTV] GetEventAsync No EVENTS for {0}| {1}", channelId, ex.Message);
              return events;
	  }
        }

        // public async Task<byte[]> GetEventImageAsync(int channelId, int eventId, int imageNumber)
        // {
        //    ProgramInfo prginfo = new ProgramInfo();
	//    prginfo = events.Get(channelId,eventId);
	//    if (prginfo.HasImage)
	//      {
        //      string url = string.Format("{0}/{1}", prginfo.ImageUrl, imageNumber);
        //      byte[] imgData = url.GetBytesFromUrl();
        //      return imgData;
	//      }
	//    else
	//      {
	//      return null;
	//      }
	// }


        public async Task<List<RecordingInfo>> GetRecordingsAsync()
        {
            List<RecordingInfo> recordings = new List<RecordingInfo>();

            GetRecordingsResponse response = await client.GetAsync(new GetRecordingsRequest());

            if (response.Recordings != null)
            {
                foreach (VdrRecording recording in response.Recordings)
                {
                    recordings.Add(new RecordingInfo()
                    {
                        Id = recording.number.ToString(),
                        ChannelId = recording.channel_id,
                        Name = recording.event_title,
                        EpisodeTitle = recording.event_short_text,
                        Overview = recording.event_description,
                        StartDate = Utils.UnixTimeStampToDateTime(recording.event_start_time),
                        EndDate = Utils.UnixTimeStampToDateTime(recording.event_start_time + recording.event_duration)
                    });
                }
            }

            return recordings;
        }

        public async Task DeleteRecording(string recordingId)
        {
            await client.DeleteAsync(new DeleteRecordingRequest() { Number = recordingId });
        }

        public async Task<List<TimerInfo>> GetTimersAsync()
        {
            List<TimerInfo> timers = new List<TimerInfo>();

            GetTimersResponse response = await client.GetAsync(new GetTimersRequest());

            if (response.timers != null)
            {
                foreach (VdrTimer timer in response.timers)
                {
                    RecordingStatus status = RecordingStatus.New;

		    if (timer.is_recording)
                        status = RecordingStatus.InProgress;

                    timers.Add(new TimerInfo()
                    {
                        Id = timer.id,
                        ChannelId = timer.channel,
                        StartDate = DateTime.Parse(timer.start_timestamp).ToUniversalTime(),
                        EndDate = DateTime.Parse(timer.stop_timestamp).ToUniversalTime(),
                        Name = timer.filename,
                        Priority = timer.priority,
                        ProgramId = timer.event_id.ToString(),
                        Status = status
                    });
                }
            }

            return timers;
        }

        public async Task CreateTimer(TimerInfo info)
        {
            DateTime startTime = info.StartDate.AddSeconds(info.PrePaddingSeconds * -1);
            DateTime endTime = info.EndDate.AddSeconds(info.PostPaddingSeconds);

            CreateTimerResponse response = await client.PostAsync(new CreateTimerRequest
            {
                file = info.Name,
                channel = info.ChannelId,
                eventid = info.ProgramId,
                start = int.Parse(startTime.ToLocalTime().ToString("HHmm")),
                stop = int.Parse(endTime.ToLocalTime().ToString("HHmm")),
                day = startTime.ToString("yyyy-MM-dd"),
                weekdays = "-------",
            });
        }

        public async Task UpdateTimer(TimerInfo info)
        {
            DateTime startTime = info.StartDate.AddSeconds(info.PrePaddingSeconds * -1);
            DateTime endTime = info.EndDate.AddSeconds(info.PostPaddingSeconds);

            UpdateTimerResponse response = await client.PutAsync(new UpdateTimerRequest
            {
                timer_id = info.Id,
                file = info.Name,
                channel = info.ChannelId,
                start = int.Parse(startTime.ToLocalTime().ToString("HHmm")),
                stop = int.Parse(endTime.ToLocalTime().ToString("HHmm")),
                day = startTime.ToString("yyyy-MM-dd"),
                weekdays = "-------",
            });
        }

        public async Task CancelTimer(string timerId)
        {
            await client.DeleteAsync(new DeleteTimerRequest() { TimerId = timerId });
        }
    }
}
