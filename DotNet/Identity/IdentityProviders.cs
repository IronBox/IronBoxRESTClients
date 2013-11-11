using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum IdentityProviders : int
    {
        Native = 0,
        Microsoft,
        Google,
        Yahoo,
        Facebook,
        None,

        // Add future identity providers, keep order
    }
}
