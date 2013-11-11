using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class EntityContainerDeleteQueueEntry
    {
        public long DeleteQueueID { set; get; }
        public long ContainerID { set; get; }
        public DateTime DeleteOnDateTimeUtc { set; get; }
    }
}
