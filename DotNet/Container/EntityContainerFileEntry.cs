using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using LockBox.Common.Security.Cryptography;
using LockBox.Common.Security.Hash;
using LockBox.Common;
using LockBox.Common.IO;

namespace LockBox
{
    public class EntityContainerBlobEntry
    {
        public String BlobID { set; get; }
        public String EncryptedBlobNameBase64 { set; get; }
        public long SizeBytes { set; get; }
        public DateTime UtcDateLastModified { set; get; }
        public LockBoxBlobStatusKey Status { set; get; }
        public String StatusData { set; get; }

        public String FileNameHash { set; get; }

        // Issue #86
        public long FileIndexID { set; get; }

        private String m_BlobName;
        public String BlobName
        {
            get
            {
                return (m_BlobName);
            }
        }

        public EntityContainerBlobEntry()
        {
            SizeBytes = 0;
            Status = LockBoxBlobStatusKey.None;
            UtcDateLastModified = DateTime.UtcNow;
            EncryptedBlobNameBase64 = String.Empty;
            BlobID = String.Empty;
            m_BlobName = String.Empty;
        }


        public bool DecryptBlobName(SymmetricAlgorithm SA)
        {
            m_BlobName = String.Empty;
            try
            {
                
                m_BlobName = SymmetricEncryptionHelper.DecryptBase64StringValue(SA, EncryptedBlobNameBase64);
                return (true);
            }
            catch (Exception)
            {
                return (false);
            }
        }


        public void SetFileNameHash(String BlobName, String ContainerFileNameSalt)
        {
            FileNameHash = CreateFileNameHash(BlobName, ContainerFileNameSalt);
        }

        public static String CreateFileNameHash(String BlobName, String ContainerFileNameSalt)
        {
            return (StringHelper.GetHexString(HashHelper.ComputeHash(StreamHelper.GetByteBufferFromString(BlobName + ContainerFileNameSalt), HashCryptoAlgorithm.SHA256)).ToLower());
        }
    }
}
