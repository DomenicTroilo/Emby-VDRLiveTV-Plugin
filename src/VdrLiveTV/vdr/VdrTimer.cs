using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.vdr
{
    public class VdrTimer
    {
        public string id { get; set; }
        public int flags { get; set; }
        public int start { get; set; }
        public string start_timestamp { get; set; }
        public int stop { get; set; }
        public string stop_timestamp { get; set; }
        public int priority { get; set; }
        public int lifetime { get; set; }
        public int event_id { get; set; }
        public string weekdays { get; set; }
        public string day { get; set; }
        public string channel { get; set; }
        public string filename { get; set; }
        public string channel_name { get; set; }
        public bool is_pending { get; set; }
        public bool is_recording { get; set; }
        public bool is_active { get; set; }
        public string aux { get; set; }
    }
}
