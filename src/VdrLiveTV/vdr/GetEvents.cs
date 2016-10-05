using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.vdr
{
    [Route("/events/{channel}", "GET")]
    public class GetEventsRequest : IReturn<GetEventsResponse>
    {
        public string channel { get; set; }
        public string timespan { get; set; }
    }

    public class GetEventsResponse
    {
        public List<VdrEvent> Events { get; set; }
    }
}
