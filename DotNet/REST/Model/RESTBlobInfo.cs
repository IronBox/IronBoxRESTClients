using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class RESTBlobInfo
    {
        public String BlobID { set; get; }
        public String BlobName { set; get; }



        /*
        public RESTBlobInfo Duplicate()
        {
            RESTBlobInfo NewObj = new RESTBlobInfo();
            NewObj.BlobID = String.Copy(this.BlobID);
            NewObj.BlobName = String.Copy(this.BlobName);
            return (NewObj);
        }
         */
    }
}
