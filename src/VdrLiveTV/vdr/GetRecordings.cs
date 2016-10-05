using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.vdr
{
    [Route("/recordings.json", "GET")]
    public class GetRecordingsRequest : IReturn<GetRecordingsResponse>
    {
    }

    public class GetRecordingsResponse
    {
        public List<VdrRecording> Recordings { get; set; }
    }
}
