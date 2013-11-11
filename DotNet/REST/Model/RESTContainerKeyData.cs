using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common;
using LockBox.Common.IO;
using LockBox.Common.Security.Cryptography;
using System.Security.Cryptography;

namespace LockBox
{
    public class RESTContainerKeyData
    {
        
        public String SessionKeyBase64 { set; get; }
        public String SessionIVBase64 { set; get; }
        public int SymmetricKeyStrength { set; get; }


        public RESTContainerKeyData()
        {

        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the symmetric key strength and algorithm of the session 
        ///     key
        /// </summary>
        /// <returns>
        ///     Returns the symmetric key strength and algorithm of the session
        ///     key
        /// </returns>
        //---------------------------------------------------------------------
        public SymmetricKeyStrength GetSymmetricKeyStrength()
        {
            return ((SymmetricKeyStrength)this.SymmetricKeyStrength);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Returns the container symmetric IV
        /// </summary>
        /// <returns>
        ///     Symmetric IV, otherwise null on error
        /// </returns>
        //---------------------------------------------------------------------
        public byte[] GetSymmetricIV()
        {
            byte[] Result = null;
            if (!String.IsNullOrEmpty(SessionIVBase64))
            {
                Result = StreamHelper.GetByteBufferFromBase64EncodedString(SessionIVBase64);
            }
            return (Result);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the container symmetric key
        /// </summary>
        /// <returns>
        ///     Symmetric key, otherwise null on error
        /// </returns>
        //---------------------------------------------------------------------
        public byte[] GetSymmetricKey()
        {
            byte[] Result = null;
            if (!String.IsNullOrEmpty(SessionKeyBase64))
            {
                Result = StreamHelper.GetByteBufferFromBase64EncodedString(SessionKeyBase64);
            }
            return (Result);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Resets the pertinent key data
        /// </summary>
        //---------------------------------------------------------------------
        public void Reset()
        {
            SessionIVBase64 = String.Empty;
            SessionKeyBase64 = String.Empty;
        }
    }
}
