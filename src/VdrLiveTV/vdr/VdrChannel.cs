using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.vdr
{
    public class VdrChannel
    {
        public string name { get; set; }
        public int number { get; set; }
        public string channel_id { get; set; }
        public bool image { get; set; }
        public string group { get; set; }
        public string transponder { get; set; }
        public string stream { get; set; }
        public bool is_atsc { get; set; }
        public bool is_cable { get; set; }
        public bool is_terr { get; set; }
        public bool is_sat { get; set; }
        public bool is_radio { get; set; }
    }
}
