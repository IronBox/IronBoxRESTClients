using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common.Security.Authentication;
using LockBox.Common;

namespace LockBox
{
    public class EntityData
    {
        public long EntityID { set; get; }
        public String NameIdentifier { set; get; }
        public AuthenticationMethod AuthMethod { set; get; }
        public String DisplayName { set; get; }

        private String m_EmailAddress;
        public String EmailAddress {
            set
            {
                m_EmailAddress = StringHelper.NormalizeEmail(value);
            }
            get
            {
                return (m_EmailAddress);
            }
        }
        public bool EmailAddressVerified { set; get; }
        public DateTime CreatedDateUTC { set; get; }
        public bool Enabled { set; get; }

        public EntityData()
        {
            EntityID = -1;
            CreatedDateUTC = DateTime.UtcNow;
            Enabled = false;
        }
    }
}
