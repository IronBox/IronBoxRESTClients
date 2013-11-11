using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class EntityQuickListData
    {
        public Dictionary<EntityQuickListDataType, String> Values = null;

        public EntityQuickListData()
        {
            Values = new Dictionary<EntityQuickListDataType, string>();
        }

        public String Name { set; get; }
        public long ListID { set; get; }
        public long OwnerID { set; get; }
    }
}
