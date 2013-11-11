using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class EntityContainerContextMembership
    {

        public long ContextID { set; get; }
        public long ContainerID { set; get; }


        public EntityContainerContextMembership()
        {
            ContainerID = -1;
            ContextID = -1;
        }
    }
}
