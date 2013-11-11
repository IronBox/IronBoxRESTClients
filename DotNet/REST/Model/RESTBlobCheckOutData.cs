using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class RESTBlobCheckOutData
    {
        public String SharedAccessSignature { set; get; }
        public String SharedAccessSignatureUri { set; get; }
        public String CheckInToken { set; get; }
        public String StorageUri { set; get; }
        public int StorageType { set; get; }
        public String ContainerStorageName { set; get; }
    }
}
