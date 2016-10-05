using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.vdr
{
    public class VdrEvent
    {
        public int id { get; set; }
        public string title { get; set; }
        public string short_text { get; set; }
        public string description { get; set; }
        public int start_time { get; set; }
        public string channel { get; set; }
        public string channel_name { get; set; }
        public int duration { get; set; }
        public int table_id { get; set; }
        public int version { get; set; }
        public int images { get; set; }
        public bool timer_exists { get; set; }
        public bool timer_active { get; set; }
        public string timer_id { get; set; }
        public int parental_rating { get; set; }
        public int vps { get; set; }
    }
}
