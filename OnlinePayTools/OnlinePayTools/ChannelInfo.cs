using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlinePayTools
{
    class ChannelInfo
    {
        string sysId;

        public string SysId
        {
            get { return sysId; }
            set { sysId = value; }
        }
        string sysName;

        public string SysName
        {
            get { return sysName; }
            set { sysName = value; }
        }

        public ChannelInfo(string sysId, string sysName) {
            this.sysId = sysId;
            this.sysName = sysName;
        }
    }
}
