using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class RESTEntityOneTimeTokenData
    {
        public String TokenID { set; get; }
        public String TokenValue { set; get; }
        public int MinutesTTL { set; get; }
    }
}
