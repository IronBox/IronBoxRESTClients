using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common.Security.Cryptography;
using System.IO;
using System.Security.Cryptography;
using LockBox.Common;
using LockBox.Common.IO;
using LockBox.Common.Security.Hash;

namespace LockBox
{
    public partial class LockBoxEntityHelper
    {

        //---------------------------------------------------------------------
        /// <summary>
        ///     Computes the SHA256 hash of the entity password + salt, 
        ///     converted into a hex string
        /// </summary>
        /// <param name="Password">Password</param>
        /// <param name="Salt">Salt</param>
        /// <returns>
        ///     Returns the hex representation of the SHA256 hash of the 
        ///     combined password and salt.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if any of the given parameters are null or empty
        /// </exception>
        //---------------------------------------------------------------------
        public static String HashEntityPasswordAsSHA256HexString(String Password, byte[] Salt)
        {
            if (String.IsNullOrEmpty(Password) || StreamHelper.ByteBufferIsNullOrEmpty(Salt))
            {
                throw new ArgumentNullException("Password or salt was null/empty");
            }

            // Combine the password and salt and take the sha256 hash of it, and
            // convert it into a hex string
            String Result = StringHelper.GetHexString(HashHelper.ComputeHash(StreamHelper.GetByteBufferFromString(Password + StringHelper.GetHexString(Salt)), HashCryptoAlgorithm.SHA256));
            return (Result);
        }
    }
}
