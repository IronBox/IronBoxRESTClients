using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum LockBoxEntitySetting : long
    {
        // Deprecated
        // Entity setting keys
        PreferredTimeZoneID = 0,
        DefaultSymmetricKeyStrength,
        DefaultAsymmetricKeyStrength,

        



        // All future setting keys, keep order consistent!
    }
}
