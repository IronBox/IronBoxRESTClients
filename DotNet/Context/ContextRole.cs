using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    // Issue #122, move the original ICO context roles into the core manager library

    public enum ContextRole
    {
        CreateDataExchangeContainers,
        WriteContainerMessages,
        LogContainerActivity,
        CreateAPIKeys,

        // Mail client data leak guard user
        DataLeakGuardUser,              // IronBox for Outlook paid user
        //DataLeakGuardUserProPlus,       // IronSight ProPlus
    }
}
