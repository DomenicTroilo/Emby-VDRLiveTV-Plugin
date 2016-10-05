using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.vdr
{
    [Route("/channels.json", "GET")]
    public class GetChannelsRequest : IReturn<GetChannelsResponse>
    {

    }

    public class GetChannelsResponse
    {
        public List<VdrChannel> Channels { get; set; }
    }
}
