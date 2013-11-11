using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum RESTMailClientContainerType : int
    {
        SendOnly,
        ReceiveOnly,
        SendAndReceive,
    }
}
