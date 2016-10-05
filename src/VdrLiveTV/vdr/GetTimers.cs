using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.vdr
{
    [Route("/timers.json", "GET")]
    public class GetTimersRequest : IReturn<GetTimersResponse>
    {
    }

    public class GetTimersResponse
    {
        public List<VdrTimer> timers { get; set; }
    }
}
