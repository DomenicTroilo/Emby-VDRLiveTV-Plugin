using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.vdr
{
    public class VdrRecording
    {
        public int number { get; set; }
        public string name { get; set; }
        public string relative_file_name { get; set; }
        public bool is_new { get; set; }
        public bool is_edited { get; set; }
        public bool is_pes_recording { get; set; }
        public int duration { get; set; }
        public int filesize_mb { get; set; }
        public string channel_id { get; set; }
        public int frames_per_second { get; set; }

        public string event_title { get; set; }
        public string event_short_text { get; set; }
        public string event_description { get; set; }
        public long event_start_time { get; set; }
        public int event_duration { get; set; }
    }
}
