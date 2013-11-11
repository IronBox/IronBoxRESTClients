using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum EntityContainerExtendedAccessType : int
    {
        AnonymousAccess_NoPassword = 0,
        AnonymousAccess_WithPassword,
        APIAccess,
    }
}
