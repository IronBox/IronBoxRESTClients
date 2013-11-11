using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Collections;
using System.Collections.Specialized;
using LockBox.Common.Security.Cryptography;
using LockBox.Common.IO;
using System.IO.Compression;
using LockBox.Common.IO.Compression;
using LockBox.Common;
using LockBox.Common.Security.Hash;
using System.Diagnostics;
using System.Security.Cryptography;

namespace LockBox
{
    public partial class LockBoxBlobHelper
    {
        public static int Default_BlobIDLength = 8;

        //---------------------------------------------------------------------
        /// <summary>
        ///     Creates a new blob ID
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static String CreateNewBlobID()
        {
            //return (NormalizeBlobID(Guid.NewGuid().ToString().Replace("-", String.Empty).Substring(0, Default_BlobIDLength)));

            // Rather than just using 8 characters of a guid, we build our ID in the following way
            String UtcNowString = String.Empty;
            foreach (char c in DateTime.UtcNow.ToString())
            {
                if (Char.IsLetterOrDigit(c))
                {
                    UtcNowString += c;
                }
            }
            return (NormalizeBlobID(String.Format("lbx{0}_{1}", UtcNowString, Guid.NewGuid().ToString().Replace("-", String.Empty).Substring(0, Default_BlobIDLength))));
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Normalizes the given blob ID
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static String NormalizeBlobID(String s)
        {
            // Must be lower case
            String Result = s.ToLower();

            // Any other transformations
            Result = Result.Trim();

            return (Result);
        }


        public static bool DropBlob(ILockBoxStorage StorageObject,
            String ContainerName, String BlobName, bool RemoveSnapshots)
        {
            // Input validation, container name and blob name will be implicitly checked
            // by the storage object
            if (StorageObject == null)
            {
                return (false);
            }
            return (StorageObject.RemoveBlob(ContainerName, BlobName, RemoveSnapshots));
        }


        //---------------------------------------------------------------------
        /// <summary>
        ///     Forms the stream name for a given blob and stream type
        /// </summary>
        /// <param name="BlobID"></param>
        /// <param name="ThisStream"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static String FormBlobStreamName(String BlobID, LockBoxBlobSubStream ThisStream)
        {
            return (String.Format("{0}.{1}", BlobID, ThisStream.ToString()));
        }


        public static bool BlobMetaDataContainsKeyWithValue(NameValueCollection MetaData, String KeyToCheck, out String Value)
        {
            Value = String.Empty;
            try
            {
                bool Result = false;
                if (MetaData != null)
                {
                    // Check if the key exists, and set the value
                    Value = MetaData[KeyToCheck];
                    Result = !String.IsNullOrEmpty(Value);
                }
                return (Result);
            }
            catch (Exception)
            {
                return (false);
            }
        }
    }
}
