using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class EntityContextMembership
    {

        public long ContextID { set; get; }
        public long EntityID { set; get; }
        public bool Enabled { set; get; }

        public EntityContextMembership()
        {
            ContextID = -1;
            EntityID = -1;
            Enabled = false;
        }
    }
}
