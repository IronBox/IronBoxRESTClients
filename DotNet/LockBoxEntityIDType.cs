using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace LockBox
{
    
    public enum LockBoxEntityIDType : int
    {
        EmailAddress = 0,
        NameIdentifier,
        ID,
    }
}
