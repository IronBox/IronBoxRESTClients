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


namespace LockBox
{
    public partial class LockBoxBlobHelper
    {


        /*
        public static bool BlobIsSymmetricallyEncrypted(NameValueCollection MetaData)
        {
            String Value;
            bool Result = false;


            if (BlobMetaDataContainsKeyWithValue(MetaData, LockBoxBlobMetaDataKey.CompressionAlgorithm.ToString(), out Value))
            {
                SymmetricKeyStrength ParsedSymmetricKeyAlgorithm;
                if (Enum.TryParse(Value, out ParsedSymmetricKeyAlgorithm))
                {
                    Result = ParsedSymmetricKeyAlgorithm != SymmetricKeyStrength.None;
                }
            }

            return (Result);
        }
        */

        
    }
}
