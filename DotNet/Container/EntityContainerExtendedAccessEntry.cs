using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LockBox.Common.Security.Cryptography;
using LockBox.Common.Security.Hash;
using LockBox.Common.IO;
using LockBox.Common;
using System.Security.Cryptography;

using System.Diagnostics;

namespace LockBox
{
    public class EntityContainerExtendedAccessEntry
    {
        public long ExtendedAccessID { set; get; }
        public long ContainerID { set; get; }
        public EntityContainerExtendedAccessType AccessType { set; get; }
        public int KeyDerivationIterations { set; get; }
        public LockBoxContainerRightsCollection Rights { set; get; }
        public byte[] ProtectedContainerSessionKey { set; get; }
        public String AccessorUserName { set; get; }
        public byte[] AccessorPasswordHash { set; get; }
        public byte[] ProtectionSalt { set; get; }
        public String AccessName { set; get; }
        public bool Enabled { set; get; }


        private byte[] m_EmptyBuffer = new byte[] { 0x0, 0x1, 0x2, 0x3 };

        public EntityContainerExtendedAccessEntry()
        {
            ExtendedAccessID = -1;
            ContainerID = -1;
            AccessType = EntityContainerExtendedAccessType.AnonymousAccess_WithPassword;
            KeyDerivationIterations = KeyDerivationHelper.GetCryptoRandomKeyIterations();
            Rights = new LockBoxContainerRightsCollection();
            ProtectedContainerSessionKey = m_EmptyBuffer;
            AccessorUserName = String.Empty;
            AccessorPasswordHash = m_EmptyBuffer;
            ProtectionSalt = m_EmptyBuffer;
            AccessName = String.Empty;
        }


        //---------------------------------------------------------------------
        /// <summary>
        ///     Validates if the given password is correct
        /// </summary>
        /// <param name="ExtendedAccessPassword">
        ///     Password
        /// </param>
        /// <returns>
        ///     Returns true if the password given is correct, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool ValidateExtendedAccess(String ExtendedAccessPassword)
        {
            bool Result = false;
            switch (AccessType)
            {
                case EntityContainerExtendedAccessType.AnonymousAccess_NoPassword:
                    // If we're anonymous with no password, then we accept null or empty
                    if (String.IsNullOrEmpty(ExtendedAccessPassword))
                    {
                        Result = true;
                    }
                    break;

                case EntityContainerExtendedAccessType.APIAccess:
                case EntityContainerExtendedAccessType.AnonymousAccess_WithPassword:
                    // Salt the given password
                    if (!String.IsNullOrEmpty(ExtendedAccessPassword))
                    {
                        String SaltedPassword = m_CreateSaltedPassword(ExtendedAccessPassword, ProtectionSalt);
                        byte[] SaltedPasswordBuffer = StreamHelper.GetByteBufferFromString(SaltedPassword);
                        byte[] SaltedPasswordHash = HashHelper.ComputeHash(SaltedPasswordBuffer, HashCryptoAlgorithm.SHA256);
                        Result = StreamHelper.ByteBuffersAreEqual(SaltedPasswordHash, AccessorPasswordHash, true);
                    }
                    break;
            }
            return (Result);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Unlocks the container session key with the given IV, strength 
        ///     and password.  Does not overwrite the protection container
        ///     session key member
        /// </summary>
        /// <param name="ContainerIV"></param>
        /// <param name="ContainerKeyStrength"></param>
        /// <param name="Password"></param>
        /// <param name="ClearTextContainerSessionKey"></param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool UnlockContainerSessionKey(EntityContainer ParentContainer, String Password, out byte[] ClearTextContainerSessionKey)
        {
            try
            {
                // If we're using anonymous no password access, the container session key is
                // the same as the protected container session key
                if (AccessType == EntityContainerExtendedAccessType.AnonymousAccess_NoPassword)
                {
                    // For anonymous with no password, password must always be null or empty
                    if (!String.IsNullOrEmpty(Password))
                    {
                        throw new Exception("Anonymous mode with no password, however password was not null or empty");
                    }
                    ClearTextContainerSessionKey = ProtectedContainerSessionKey;
                    return (true);
                }

                // Otherwise we're using a protected method and derive the key
                byte[] ProtectionKey = KeyDerivationHelper.DeriveSymmetricKey(ParentContainer.SymmetricProtection, KeyDerivationIterations, ProtectionSalt, Password);
                SymmetricAlgorithm SA = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(ParentContainer.SymmetricProtection);
                SA.Key = ProtectionKey;
                SA.IV = ParentContainer.SymmetricIV;

                // Decrypt the protected key
                ClearTextContainerSessionKey = SymmetricEncryptionHelper.Symmetric_EncryptOrDecrypt(ProtectedContainerSessionKey, SA, CryptoMode.Decrypt);
                if (ClearTextContainerSessionKey == null)
                {
                    throw new Exception("Unable to decrypt the container session key");
                }

                // Done, 
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("EntityContainerExtendedAccessEntry->UnlockContainerSessionKey", e.Message);
                ClearTextContainerSessionKey = null;
                return (false);
            }
        }


        
        public bool LockContainerSessionKey(byte[] ClearTextContainerSessionKey, EntityContainer ParentContainerData, String PasswordToUse)
        {

            try
            {
                // Input validation
                if (ClearTextContainerSessionKey == null)
                {
                    throw new Exception("Container session key is null");
                }
                if (ParentContainerData == null)
                {
                    throw new Exception("Parent container object is null");
                }
                if (ParentContainerData.SymmetricIV == null)
                {
                    throw new Exception("Container IV is null");
                }
                if (String.IsNullOrEmpty(PasswordToUse) && (AccessType != EntityContainerExtendedAccessType.AnonymousAccess_NoPassword))
                {
                    throw new Exception("Password is required for access type " + AccessType.ToString());
                }

                // Defaults
                AccessorPasswordHash = m_EmptyBuffer;

                // Derive protection variables
                byte[] ProtectionKey = null;
                String ProtectionSaltStr = Guid.NewGuid().ToString();
                ProtectionSalt = StreamHelper.GetByteBufferFromString(ProtectionSaltStr);
                KeyDerivationIterations = KeyDerivationHelper.GetCryptoRandomKeyIterations();

                // The encrypted protection key
                ProtectedContainerSessionKey = null;

                // Prep based on the access type we're creating
                switch (AccessType)
                {
                    case EntityContainerExtendedAccessType.AnonymousAccess_NoPassword:
                        // We store the container session key in clear text
                        ProtectedContainerSessionKey = ClearTextContainerSessionKey;
                        break;

                    // Essentially these two are the same
                    case EntityContainerExtendedAccessType.AnonymousAccess_WithPassword:
                    case EntityContainerExtendedAccessType.APIAccess:
                        // Derive a protection key from the password given
                        ProtectionKey = KeyDerivationHelper.DeriveSymmetricKey(ParentContainerData.SymmetricProtection, KeyDerivationIterations, ProtectionSalt, PasswordToUse);
                        if (ProtectionKey == null)
                        {
                            throw new Exception("Unable to create protection key");
                        }
                        break;
                }

                // Encrypt the container session key
                if (ProtectionKey != null)
                {
                    LockBoxDebugHelper.Debug_Log("EntityContainerExtendedAccessEntry->ProtectContainerSessionKey", "Encrypting container session key", false);
                    // Create the encrypting object
                    SymmetricAlgorithm SA = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(ParentContainerData.SymmetricProtection);
                    SA.Key = ProtectionKey;
                    SA.IV = ParentContainerData.SymmetricIV;
                    ProtectedContainerSessionKey = SymmetricEncryptionHelper.Symmetric_EncryptOrDecrypt(ClearTextContainerSessionKey, SA, CryptoMode.Encrypt);

                    // Save the salted SHA512 hash of the password
                    String SaltedPassword = m_CreateSaltedPassword(PasswordToUse, ProtectionSaltStr);
                    AccessorPasswordHash = HashHelper.ComputeHash(StreamHelper.GetByteBufferFromString(SaltedPassword), HashCryptoAlgorithm.SHA256);
                }

                // And we're done
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("EntityContainerExtendedAccessEntry->ProtectContainerSessionKey", e.Message);
                return (false);
            }
        }


        private String m_CreateSaltedPassword(String Password, byte[] Salt)
        {
            return (m_CreateSaltedPassword(Password, StringHelper.GetString(Salt)));
        }

        private String m_CreateSaltedPassword(String Password, String Salt)
        {
            return (String.Format("{0}{1}", Password, Salt));
        }
    }
}
