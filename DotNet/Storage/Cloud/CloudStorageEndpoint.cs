using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class CloudStorageEndpoint
    {
        public long EndpointID { set; get; }
        public String Name { set; get; }
        public String AccountName { set; get; }
        public String AccountKey { set; get; }
        public String AccountUrl { set; get; }
        public PhysicalStorageLocation Location { set; get; }
        public CloudStorageVendor Vendor { set; get; }
        public bool IsPrimary { set; get; }
        public bool Enabled { set; get; }


        public CloudStorageEndpoint()
        {
            EndpointID = -1;
            Name = String.Empty;
            AccountName = String.Empty;
            AccountKey = String.Empty;
            AccountUrl = String.Empty;
            Location = PhysicalStorageLocation.Local;
            Vendor = CloudStorageVendor.Microsoft;
            IsPrimary = false;
            Enabled = false;
        }
    }
}
