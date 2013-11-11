using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common.Security.Cryptography;
using System.Security.Cryptography;
using LockBox.Common.Data;
using LockBox.Common;
using LockBox.Common.IO;

namespace LockBox
{
    // Issue #22: One time token accessor

    public class EntityOneTimeTokenData
    {
        public String TokenID { set; get; }
        public long EntityID { set; get; }
        public SymmetricKeyStrength KeyStrength { set; get; }
        public byte[] IV { set; get; }
        public int KeyDerivationIterations { set; get; }
        public byte[] Salt { set; get; }
        public byte[] ProtectedPassword { set; get; }
        public DateTime DateCreatedUtc { set; get; }

        public const int DefaultMinutesTTL = 3;

        //---------------------------------------------------------------------
        /// <summary>
        ///     Constructor to add defaults
        /// </summary>
        //---------------------------------------------------------------------
        public EntityOneTimeTokenData()
        {
            TokenID = null;
            EntityID = -1;
            KeyStrength = SymmetricKeyStrength.AES256;
            
            ProtectedPassword = null;
            DateCreatedUtc = DateTime.UtcNow;

            // Create new key material
            CreateNewKeyMaterial(KeyStrength);
        }


        //---------------------------------------------------------------------
        /// <summary>
        ///     Creates new key material
        /// </summary>
        /// <param name="KeyStrength"></param>
        //---------------------------------------------------------------------
        public void CreateNewKeyMaterial(SymmetricKeyStrength KeyStrength)
        {
            this.KeyStrength = KeyStrength;
            SymmetricAlgorithm SA = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(KeyStrength);
            IV = SA.IV;
            Salt = StreamHelper.GetByteBufferFromString(Guid.NewGuid().ToString());
            KeyDerivationIterations = KeyDerivationHelper.GetCryptoRandomKeyIterations();
        }

        private String m_CreateTokenValue()
        {
            return (Guid.NewGuid().ToString().Replace("-", string.Empty));
        }

        public bool LockPassword(String PasswordToProtect, out String TokenValue)
        {
            try
            {
                // Validate that the password is not null or empty
                if (String.IsNullOrEmpty(PasswordToProtect))
                {
                    throw new Exception("Password was empty or null");
                }

                // Encrypt the password
                TokenValue = m_CreateTokenValue();
                SymmetricAlgorithm SA = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(KeyStrength);
                SA.Key = KeyDerivationHelper.DeriveSymmetricKey(KeyStrength, KeyDerivationIterations, Salt, TokenValue);
                SA.IV = IV;

                ProtectedPassword = SymmetricEncryptionHelper.Symmetric_EncryptOrDecrypt(StreamHelper.GetByteBufferFromString(PasswordToProtect), SA, CryptoMode.Encrypt);

                // Done, no errors
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("EntityOneTimeTokenData::LockPassword", e.Message);
                TokenValue = null;
                return (false);
            }

        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Retrieves the protected password with a given token value
        /// </summary>
        /// <param name="TokenValue"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool UnlockPassword(String TokenValue, out String Password)
        {
            try
            {
                // Create an encryptor based on the key strength
                SymmetricAlgorithm SA = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(KeyStrength);

                // Create the key based on the TokenValue provided
                SA.Key = KeyDerivationHelper.DeriveSymmetricKey(KeyStrength, KeyDerivationIterations, Salt, TokenValue);
                SA.IV = IV;

                // Decrypt the protected password
                byte[] DecryptedPasswordBytes = SymmetricEncryptionHelper.Symmetric_EncryptOrDecrypt(ProtectedPassword, SA, CryptoMode.Decrypt);
                Password = StringHelper.GetString(DecryptedPasswordBytes);

                // Everything was successful, return success
                return (true);
            }
            catch (Exception)
            {
                Password = null;
                return (false);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Indicates if the token is expired using the default TTL
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool IsExpired()
        {
            // Default TTL is 3 minutes per specifications
            return (IsExpired(DefaultMinutesTTL));
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     
        /// </summary>
        /// <param name="NumMinutesTillExpires"></param>
        /// <returns>
        ///     Returns true if the token is expired, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool IsExpired(int NumMinutesTillExpires)
        {
            return (DateTime.Compare(DateTime.UtcNow, DateCreatedUtc.AddMinutes(NumMinutesTillExpires)) > 0);
        }
    }
}
