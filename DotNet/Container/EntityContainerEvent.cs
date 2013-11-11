using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum EntityContainerEvent : int
    {
        EntityCreateBlob,
        EntityRemoveBlob,
        EntityReadBlob,

        EntityCreateContainer,

        EntityWriteMessage,
        
    }
}
