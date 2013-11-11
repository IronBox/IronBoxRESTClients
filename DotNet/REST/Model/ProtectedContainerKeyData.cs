using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common.Security.Cryptography;
using System.Security.Cryptography;
using LockBox.Common.IO;


namespace LockBox
{
    public class ProtectedContainerKeyData
    {

        private byte[] m_ProtectedKey = null;
        private byte[] m_IV = null;
        private SymmetricKeyStrength m_SymmetricKeyStrength;
        private MemoryProtectionScope m_Scope = MemoryProtectionScope.SameProcess;

        public ProtectedContainerKeyData(byte[] Key, byte[] IV, SymmetricKeyStrength SKeyStrength)
        {
            if ((Key == null) || (IV == null))
            {
                throw new ArgumentNullException("Key or IV was null");
            }
            // Copy the key and protect it
            m_ProtectedKey = StreamHelper.CopyBuffer(Key);
            ProtectedMemory.Protect(m_ProtectedKey, m_Scope);
            
            // Copy the rest of the key material
            m_IV = StreamHelper.CopyBuffer(IV);
            m_SymmetricKeyStrength = SKeyStrength;
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the unprotected symmetric key
        /// </summary>
        /// <returns>
        ///     Unprotected symmetric key
        /// </returns>
        //---------------------------------------------------------------------
        public byte[] GetKey()
        {
            // Make a copy of the protected key and unprotect it
            byte[] KeyCopy = StreamHelper.CopyBuffer(m_ProtectedKey);
            ProtectedMemory.Unprotect(KeyCopy, m_Scope);
            return (KeyCopy);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the symmetric IV
        /// </summary>
        /// <returns>
        ///     Returns symmetric IV
        /// </returns>
        //---------------------------------------------------------------------
        public byte[] GetIV()
        {
            return (m_IV);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the symmetric key strength
        /// </summary>
        /// <returns>
        ///     Returns symmetric key strength
        /// </returns>
        //---------------------------------------------------------------------
        public SymmetricKeyStrength GetKeyStrength()
        {
            return (m_SymmetricKeyStrength);
        }


        public SymmetricAlgorithm GetSymmetricAlgorithmObject()
        {
            SymmetricAlgorithm SA = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(GetKeyStrength());
            SA.Key = GetKey();
            SA.IV = GetIV();
            return (SA);

        }
    }
}
