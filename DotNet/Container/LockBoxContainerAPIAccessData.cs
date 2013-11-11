using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using LockBox.Common;
using LockBox.Common.Security.Cryptography;
using LockBox.Common.IO;
using LockBox.Common.Data;

namespace LockBox
{
    public class LockBoxContainerAPIAccessData
    {

        public long ParentContainerID { set; get; }
        public String APIUserName { set; get; }
        public byte[] ProtectedContainerSessionKey { set; get; }
        public int ProtectionKeyDerivationIterations { set; get; }
        public LockBoxContainerRightsCollection Rights { set; get; }

        // Parent key materials
        public SymmetricKeyStrength ParentContainerSymmetricStrength { set; get; }
        public String ParentContainerSalt { set; get; }
        public byte[] ParentContainerSymmetricIV { set; get; }

        public LockBoxContainerAPIAccessData()
        {
            Reset();
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Loads the parent container settings into this extended access
        ///     object
        /// </summary>
        /// <param name="ParentContainerInfo">
        ///     Parent entity container object
        /// </param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool LoadParentContainerSettings(EntityContainer ParentContainerInfo)
        {
            if (ParentContainerInfo == null)
            {
                return (false);
            }

            ParentContainerID = ParentContainerInfo.ContainerID;
            ParentContainerSymmetricStrength = ParentContainerInfo.SymmetricProtection;
            ParentContainerSalt = ParentContainerInfo.FileNameSalt;
            ParentContainerSymmetricIV = ParentContainerInfo.SymmetricIV;

            // Done
            return (true);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Resets this extended access data object
        /// </summary>
        //---------------------------------------------------------------------
        public void Reset()
        {
            ParentContainerID = -1;
            ProtectedContainerSessionKey = null;
            if (Rights == null)
            {
                Rights = new LockBoxContainerRightsCollection();
            }
            Rights.Clear();
            ProtectionKeyDerivationIterations = KeyDerivationHelper.GetCryptoRandomKeyIterations();
            APIUserName = String.Empty;
        }


        

        public bool ConfigureForAPIAccess(String AccessUserNameToUse, String APIKeyToUse, byte[] UnprotectedContainerSessionKey, LockBoxContainerRightsCollection RightsToUse)
        {
            if (String.IsNullOrEmpty(AccessUserNameToUse))
            {
                LockBoxDebugHelper.Debug_Log("ConfigureForAPIAccess", "Access username cannot be null or empty", true);
                return (false);
            }
            if (String.IsNullOrEmpty(APIKeyToUse))
            {
                LockBoxDebugHelper.Debug_Log("ConfigureForAPIAccess", "Access API key cannot be null or empty", true);
                return (false);
            }
            APIUserName = AccessUserNameToUse;
            return (m_SaveAccessKeyWithRights(UnprotectedContainerSessionKey, APIKeyToUse, RightsToUse));
        }



        private bool m_SaveAccessKeyWithRights(byte[] UnprotectedContainerSessionKey, String AccessPassword, LockBoxContainerRightsCollection RightsToUse)
        {
            if (StreamHelper.ByteBufferIsNullOrEmpty(UnprotectedContainerSessionKey))
            {
                LockBoxDebugHelper.Debug_Log("LockBoxContainerExtendedAccessData->m_SaveAccessKey", "Access key was null or empty", true);
                return (false);
            }

            // Assign rights
            Rights.Clear();
            if (RightsToUse != null)
            {
                Rights.LoadDBStorableString(RightsToUse.GetDBStorageString());
                m_DropInvalidExtendedAccessRights();
            }

            ProtectedContainerSessionKey = UnprotectedContainerSessionKey;
            bool DoEncryption = !String.IsNullOrEmpty(AccessPassword);
            if (DoEncryption)
            {
                // Come up with a random key derivation size each time
                ProtectionKeyDerivationIterations = KeyDerivationHelper.GetCryptoRandomKeyIterations();

                // Convert salt into byte buffer
                byte[] Salt = StreamHelper.GetByteBufferFromString(ParentContainerSalt);

                // Derive the session key from the password
                byte[] ProtectionSessionKey = KeyDerivationHelper.DeriveSymmetricKey(ParentContainerSymmetricStrength, ProtectionKeyDerivationIterations, Salt, AccessPassword);

                // Encrypt the unprotected container session key
                SymmetricAlgorithm SA = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(ParentContainerSymmetricStrength);
                SA.Key = ProtectionSessionKey;
                SA.IV = ParentContainerSymmetricIV;
                ProtectedContainerSessionKey = SymmetricEncryptionHelper.Symmetric_EncryptOrDecrypt(UnprotectedContainerSessionKey, SA, CryptoMode.Encrypt);
            }
            return (true);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Validates the access password given and returns the unprotected
        ///     container session key if valid
        /// </summary>
        /// <param name="APIKey">API key to validate</param>
        /// <param name="UnprotectedContainerSessionKey"></param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool ValidateAPIKey(String APIKey, out byte[] UnprotectedContainerSessionKey)
        {
            // Try to decrypt the password
            try
            {
                // If a password is not needed validation, this means the copy of the container
                // session key is in clear text
                UnprotectedContainerSessionKey = ProtectedContainerSessionKey;

                byte[] Salt = StreamHelper.GetByteBufferFromString(ParentContainerSalt);
                byte[] ProtectionSessionKey = KeyDerivationHelper.DeriveSymmetricKey(ParentContainerSymmetricStrength, 
                    ProtectionKeyDerivationIterations, Salt, APIKey);

                // Decrypt the encrypted container session key
                SymmetricAlgorithm SA = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(ParentContainerSymmetricStrength);
                SA.Key = ProtectionSessionKey;
                SA.IV = ParentContainerSymmetricIV;
                UnprotectedContainerSessionKey = SymmetricEncryptionHelper.Symmetric_EncryptOrDecrypt(ProtectedContainerSessionKey, SA, CryptoMode.Decrypt);


                // If the decryption failed then an exception would have been thrown on the decryption
                return (!StreamHelper.ByteBufferIsNullOrEmpty(UnprotectedContainerSessionKey));
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("LockBoxContainerExtendedAccessData->ValidateAccessPassword", e.Message, true);
                UnprotectedContainerSessionKey = null;
                return (false);
            }
        }


        //---------------------------------------------------------------------
        /// <summary>
        ///     Adds a given right to this extended access object, only Read and Write 
        ///     allowed
        /// </summary>
        /// <param name="ThisRight"></param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public void AddRight(LockBoxContainerRights ThisRight)
        {
            Rights.AddRight(ThisRight);
            m_DropInvalidExtendedAccessRights();
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Removes a right from this object
        /// </summary>
        /// <param name="ThisRight"></param>
        //---------------------------------------------------------------------
        public void RemoveRight(LockBoxContainerRights ThisRight)
        {
            Rights.RemoveRight(ThisRight);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets a storable db rights string that represents the rights
        ///     in this object
        /// </summary>
        /// <returns>
        ///     A storable db rights string
        /// </returns>
        //---------------------------------------------------------------------
        public String GetDBStorableRightsString()
        {
            m_DropInvalidExtendedAccessRights();
            return (Rights.GetDBStorageString());
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Loads the given DB storable rights string
        /// </summary>
        /// <param name="s"></param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool LoadDBStorableRightsString(String s)
        {
            Rights.Clear();
            bool Results = Rights.LoadDBStorableString(s);
            m_DropInvalidExtendedAccessRights();
            return (Results);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Removes any invalid rights 
        /// </summary>
        //---------------------------------------------------------------------
        private void m_DropInvalidExtendedAccessRights()
        {
            if (Rights != null)
            {
                foreach (LockBoxContainerRights CurrentRight in Enum.GetValues(typeof(LockBoxContainerRights)))
                {
                    switch (CurrentRight)
                    {
                        case LockBoxContainerRights.Read:
                        case LockBoxContainerRights.Write:
                        case LockBoxContainerRights.Delete:
                            // Nop, these are valid rights
                            break;

                        default:
                            // All others get deleted
                            Rights.RemoveRight(CurrentRight);
                            break;
                    }
                }
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Returns a new API key
        /// </summary>
        /// <returns>
        ///     Returns a new API key
        /// </returns>
        //---------------------------------------------------------------------
        public static String GenerateNewAPIKey()
        {
            return (DataGenerator.CreateAPIKey());
        }
    }
}
