using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum PhysicalStorageLocation : uint
    {
        Local = 0,
        UnitedStates_Anywhere = 1,
        UnitedStates_WestCoast = 2,
        UnitedStates_EastCoast = 3,
    }
}
