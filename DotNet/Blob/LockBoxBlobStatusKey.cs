using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum LockBoxBlobStatusKey : uint
    {
        BlobCreated = 0,
        EntityUploading,
        Ready,
        CheckedOut,
        EntityModifying,
        

        // Default
        None,
    }
}
