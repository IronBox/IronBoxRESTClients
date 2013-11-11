using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common;
namespace LockBox
{
    public class UploadQueueEntry
    {
        public long UploadID { set; get; }
        public long ContainerID { set; get; }
        public long FileIndexID { set; get; }
        public String OwnerHostName { set; get; }
        public String FilePath { set; get; }
        public String BlobID { set; get; }
        public long OriginalSizeBytes { set; get; }


        public UploadQueueEntry()
        {
            UploadID = -1;
            ContainerID = -1;
            FileIndexID = -1;
            OwnerHostName = null;
            FilePath = null;
            BlobID = null;
            OriginalSizeBytes = 0;
        }


        public bool IsValid()
        {
            try
            {
                if (UploadID == -1)
                {
                    throw new Exception("Upload ID is invalid");
                }

                if (ContainerID == -1)
                {
                    throw new Exception("ContainerID is invalid");
                }

                if (FileIndexID == -1)
                {
                    throw new Exception("FileIndexID is invalid");
                }

                if (String.IsNullOrEmpty(OwnerHostName))
                {
                    throw new Exception("OwnerHostName is invalid");
                }

                if (String.IsNullOrEmpty(FilePath))
                {
                    throw new Exception("FilePath is invalid");
                }

                if (String.IsNullOrEmpty(BlobID))
                {
                    throw new Exception("BlobID is invalid");
                }

                // Passed all tests
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("UploadQueueEntry->IsValid", e.Message);
                return (false);
            }

        }
    }
}
