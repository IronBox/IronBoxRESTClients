using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum LockBoxBlobSubStream
    {
        metadata,
        data,
        
        // Add future stream names here
        id,
        idmap,
    }
}
