using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum EntityAccountActivationProperty : long
    {
        AddToContextIDsCSV = 0,
        NotifyOnActivationEntityIDsCSV,


        EnableSFT,
        EnableLogging,
        EnableSecureMessaging,
        EnableContextAdmin,

        // Issue #131, Pay as you go
        PayAsYouGoUser,


        EnableIronBoxOutlook,

        // Any further activations here
    }
}
