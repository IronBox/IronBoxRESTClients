using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class SharedAccessSignatureHelper
    {
        // Max is 60 minutes for Azure, but we pick something much lower
        public static double SharedAccessSignatureMaxTtlMinutes = 45;
    }
}
