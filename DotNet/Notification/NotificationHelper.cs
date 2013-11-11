using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class NotificationHelper
    {
        public const String IronBoxID = "IronBox-ID";

        public static String CreateIronBoxIDTag(String ContainerFriendlyID)
        {
            return (String.Format("{0}:{1}", IronBoxID, ContainerFriendlyID));
        }
    }
}
