using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum LockBoxStatisticsKey : long
    {
        Debug = -1,
        NumBytesProtected = 0,
        NumContainersCreated,
        NumConversationsProtected,
        
    }
}
