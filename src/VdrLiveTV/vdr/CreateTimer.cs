using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.vdr
{
    [Route("/timers.json", "POST")]
    public class CreateTimerRequest : IReturn<CreateTimerResponse>
    {
        public string file { get; set; }
        public int start { get; set; }
        public int stop { get; set; }
        public string weekdays { get; set; }
        public int priority { get; set; }
        public int lifetime { get; set; }
        public string eventid { get; set; }
        public string day { get; set; }
        public string channel { get; set; }
    }

    public class CreateTimerResponse
    {
        public List<VdrTimer> timers { get; set; }
    }
}
