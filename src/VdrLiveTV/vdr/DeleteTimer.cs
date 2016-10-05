using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.vdr
{
    [Route("/timers/{TimerId}", "DELETE")]
    public class DeleteTimerRequest : IReturnVoid
    {
        public string TimerId { get; set; }
    }
}
