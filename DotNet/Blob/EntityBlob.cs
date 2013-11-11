using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class EntityBlob
    {
        public long ContainerID { set; get; }
        public String BlobName { set; get; }
        public String BlobID { set; get; }

        public String SharedAccessSignature { set; get; }
        public String SharedAccessSignatureWithUrl { set; get; }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Set by the entity manager
        /// </summary>
        //---------------------------------------------------------------------
        public String EncryptedBlobNameBase64 { set; get; }

        public String Data { set; get; }

    }
}
