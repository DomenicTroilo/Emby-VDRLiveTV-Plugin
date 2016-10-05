using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.vdr
{
    [Route("/recordings/{Number}", "DELETE")]
    public class DeleteRecordingRequest : IReturnVoid
    {
        public string Number { get; set; }
    }
}
