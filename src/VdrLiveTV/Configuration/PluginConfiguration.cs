using MediaBrowser.Model.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VdrLiveTV.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string VDR_ServerName { get; set; }
        public int VDR_HTTP_Port { get; set; }
        public int VDR_RestfulApi_Port { get; set; }

        public PluginConfiguration()
        {
            VDR_ServerName = "localhost";
            VDR_HTTP_Port = 3000;
            VDR_RestfulApi_Port = 8002;
        }
    }
}
