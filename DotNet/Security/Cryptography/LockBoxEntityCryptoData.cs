using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common;
using System.Security.Cryptography;
using LockBox.Common.Security.Cryptography;
using LockBox.Common.IO;

namespace LockBox
{
    public class LockBoxEntityCryptoData
    {

        public static bool UseOAEPPadding = false;

        //---------------------------------------------------------------------
        /// <summary>
        ///     Minimum key derivation iterations
        /// </summary>
        //---------------------------------------------------------------------
        public static int MinKeyDerivationIterations = 10000;

        //---------------------------------------------------------------------
        /// <summary>
        ///     Collection of public key material
        /// </summary>
        //---------------------------------------------------------------------
        public Dictionary<LockBoxEntityAsymmetricKey, byte[]> AsymmetricPublicKeyMaterial
        {
            set;
            get;
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Collection of private key material
        /// </summary>
        //---------------------------------------------------------------------
        public Dictionary<LockBoxEntityAsymmetricKey, byte[]> AsymmetricPrivateKeyMaterial
        {
            set;
            get;
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Sets or gets the number of key derivation iterations
        /// </summary>
        //---------------------------------------------------------------------
        public int KeyDerivationIterations { set; get; }


        public byte[] ProtectionIV { set; get; }
        public byte[] PasswordSalt { set; get; }
        public SymmetricKeyStrength ProtectionKeyStrength { set; get; }
        public bool IsProtected { set; get; }               // Value that indicates if this key material is protected or not

        //---------------------------------------------------------------------
        /// <summary>
        ///     Resets this key material object
        /// </summary>
        //---------------------------------------------------------------------
        public void Reset()
        {
            if (AsymmetricPublicKeyMaterial == null)
            {
                AsymmetricPublicKeyMaterial = new Dictionary<LockBoxEntityAsymmetricKey, byte[]>();
            }
            AsymmetricPublicKeyMaterial.Clear();

            if (AsymmetricPrivateKeyMaterial == null)
            {
                AsymmetricPrivateKeyMaterial = new Dictionary<LockBoxEntityAsymmetricKey, byte[]>();
            }
            AsymmetricPrivateKeyMaterial.Clear();

            ProtectionKeyStrength = SymmetricKeyStrength.AES256;
            KeyDerivationIterations = 0;
            ProtectionIV = null;
            IsProtected = false;
            PasswordSalt = new byte[256];
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Constructor
        /// </summary>
        //---------------------------------------------------------------------
        public LockBoxEntityCryptoData()
        {
            Reset();
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Changes the protection password of the key data
        /// </summary>
        /// <param name="OldPassword">Old password (if any)</param>
        /// <param name="NewPassword">New password</param>
        /// <returns>
        ///     Returns true on success, false otherwise.
        /// </returns>
        //---------------------------------------------------------------------
        public bool ChangeProtectionPassword(String OldPassword, String NewPassword)
        {
            if (IsProtected)
            {
                // We need to unlock the current key material first, then
                // relock with the new password
                return (UnprotectPrivateKeyMaterial(OldPassword) &&
                    ProtectPrivateKeyMaterial(NewPassword));
            }
            else
            {
                // Not protected, so we don't need the old password
                return (ProtectPrivateKeyMaterial(NewPassword));
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Unprotects the private key material using the given password
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool UnprotectPrivateKeyMaterial(String Password)
        {
            return (ModifyPrivateKeyMaterial(Password, false));
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Returns the appropriate protection key used to protect or unprotect
        ///     private key data for this object
        /// </summary>
        /// <param name="Password">Password to derive key from</param>
        /// <returns>
        ///     Returns a byte buffer that represents an appropriate key from 
        ///     which to use for protecting the data contained within this object.
        /// </returns>
        //---------------------------------------------------------------------
        public byte[] DeriveProtectionKey(String Password)
        {
            //return (LockBoxProtectData.DeriveSymmetricKey(this.ProtectionKeyStrength, KeyDerivationIterations, PasswordSalt, Password));
            return (KeyDerivationHelper.DeriveSymmetricKey(this.ProtectionKeyStrength, KeyDerivationIterations, PasswordSalt, Password));
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Protects the private key material held within this object
        ///     with the given password
        /// </summary>
        /// <param name="Password"></param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool ProtectPrivateKeyMaterial(String Password) {

            return (ModifyPrivateKeyMaterial(Password, true));
        }

        private bool ModifyPrivateKeyMaterial(String Password, bool Protect)
        {
            // We can only act on unencrypted key material, so return false if the 
            // the key data is already encrypted
            if (Protect & IsProtected)
            {
                LockBoxDebugHelper.Debug_Log("ModifyPrivateKeyMaterial", "Key material is already locked so can't re-lock it", true);
                return (false);
            }
            else if (!Protect && !IsProtected)
            {
                LockBoxDebugHelper.Debug_Log("ModifyPrivateKeyMaterial", "Key material is already unlocked so can't re-unlock it", true);
                return (false);
            }

            // Derive the protection key
            int ProtectionKeySize = GetProtectionKeySize();
            byte[] ProtectionKey = DeriveProtectionKey(Password);

            // Create space for the newly encrypted private keys
            Dictionary<LockBoxEntityAsymmetricKey, byte[]> ModifiedPrivateKeyMaterial = new Dictionary<LockBoxEntityAsymmetricKey, byte[]>();

            // Iterate through all the private keys and encrypt them
            SymmetricAlgorithm sa = GetSymmetricAlgorithmEncryptor();
            sa.KeySize = ProtectionKeySize;
            sa.Key = ProtectionKey;
            sa.IV = ProtectionIV;

            foreach (KeyValuePair<LockBoxEntityAsymmetricKey, byte[]> kvp in AsymmetricPrivateKeyMaterial)
            {

                ModifiedPrivateKeyMaterial[kvp.Key] = SymmetricEncryptionHelper.Symmetric_EncryptOrDecrypt(kvp.Value, sa, Protect ? CryptoMode.Encrypt : CryptoMode.Decrypt);
                if (ModifiedPrivateKeyMaterial[kvp.Key] == null)
                {
                    // Error, return false
                    if (Protect)
                    {
                        LockBoxDebugHelper.Debug_Log("ModifyPrivateKeyMaterial", "Unable to encrypt private key for " + kvp.Key, true);
                    }
                    else
                    {
                        LockBoxDebugHelper.Debug_Log("ModifyPrivateKeyMaterial", "Unable to decrypt private key for " + kvp.Key, true);
                    }
                    return (false);
                }
            }

            /*
            // Securely encrypt the old unprotected private keys, zero out the 
            foreach (KeyValuePair<LockBoxUserAsymmetricKey, byte[]> kvp in AsymmetricPrivateKeyMaterial)
            {
                ProtectedMemory.Protect(kvp.Value, MemoryProtectionScope.SameProcess);
            }
            */

            // Copy the new encrypted private keys over the old data structure
            AsymmetricPrivateKeyMaterial = ModifiedPrivateKeyMaterial;
            IsProtected = Protect;

            // No errors,
            return (true);
        }


        private int GetProtectionKeySize()
        {
            return (SymmetricEncryptionHelper.GetSymmetricKeySizeBits(this.ProtectionKeyStrength));
        }


        private SymmetricAlgorithm GetSymmetricAlgorithmEncryptor()
        {
            return (SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(ProtectionKeyStrength));
        }


        public bool CreateNewKeyMaterial(SymmetricKeyStrength SymmetricProtectionKeyStrength, String Password, int MaxKeyDerivationIterations)
        {
            // Clear all loaded keys
            Reset();

            //-----------------------------------------------------------------
            // Determine the protection key strength, order is very important
            // set the protection key strength first, then get the encryptor
            // then set the key size
            //-----------------------------------------------------------------
            SymmetricAlgorithm sa = null;
            this.ProtectionKeyStrength = SymmetricProtectionKeyStrength;
            sa = GetSymmetricAlgorithmEncryptor();
            if (sa == null)
            {
                LockBoxDebugHelper.Debug_Log("CreateNewKeyMaterial", "Unable to create symmetric algorithm protector", true);
                return (false);
            }
            sa.KeySize = GetProtectionKeySize();

            //-----------------------------------------------------------------
            // Determine the new IV, salt and password salt
            //-----------------------------------------------------------------
            ProtectionIV = sa.IV;
            RNGCryptoServiceProvider CRNG = new RNGCryptoServiceProvider();
            CRNG.GetBytes(PasswordSalt);

            //-----------------------------------------------------------------
            // Generate a random number of key iterations for key 
            // derivation
            //-----------------------------------------------------------------
            /*CryptoRandom crng = new CryptoRandom();
            KeyDerivationIterations = crng.Next(MinKeyDerivationIterations, MaxKeyDerivationIterations);
             */
            KeyDerivationIterations = KeyDerivationHelper.GetCryptoRandomKeyIterations(MaxKeyDerivationIterations);

            //-----------------------------------------------------------------
            // Generate the asymmetric keys, do not protect them yet
            //-----------------------------------------------------------------
            try
            {
                foreach (AsymmetricKeyStrength AKeyStrength in Enum.GetValues(typeof(AsymmetricKeyStrength)))
                {
                    switch (AKeyStrength)
                    {
                        case AsymmetricKeyStrength.RSA_1024:
                        case AsymmetricKeyStrength.RSA_2048:
                        case AsymmetricKeyStrength.RSA_3072:
                        case AsymmetricKeyStrength.RSA_15360:
                            m_CreateAndSaveUnprotectedRSAKeyMaterial(AKeyStrength);
                            break;

                        default:
                            throw new NotImplementedException("Asymmetric algorithm not implemented, " + AKeyStrength.ToString());

                    }
                }

                // Encrypt the private key material
                if (!ProtectPrivateKeyMaterial(Password))
                {

                    throw new CryptographicException("Unable to protect private/public key data");
                }

                // Done, return true
                return (true);
            }
            catch (Exception e)
            {
                // Error, clear all asymmetric key material
                Reset();
                LockBoxDebugHelper.Debug_Log("CreateNewKeyMaterial", e.Message, true);
                return (false);
            }
        }

        internal void m_CreateAndSaveUnprotectedRSAKeyMaterial(AsymmetricKeyStrength ThisAsymmetricKeyStrength)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            LockBoxEntityAsymmetricKey PublicKeyIndex = LockBoxEntityAsymmetricKey.RSA_1024_Public;
            LockBoxEntityAsymmetricKey PrivateKeyIndex = LockBoxEntityAsymmetricKey.RSA_1024_Private;

            int KeyStrength;
            switch (ThisAsymmetricKeyStrength)
            {
                case AsymmetricKeyStrength.RSA_1024:
                    KeyStrength = 1024;
                    PublicKeyIndex = LockBoxEntityAsymmetricKey.RSA_1024_Public;
                    PrivateKeyIndex = LockBoxEntityAsymmetricKey.RSA_1024_Private;
                    break;

                case AsymmetricKeyStrength.RSA_2048:
                    KeyStrength = 2048;
                    PublicKeyIndex = LockBoxEntityAsymmetricKey.RSA_2048_Public;
                    PrivateKeyIndex = LockBoxEntityAsymmetricKey.RSA_2048_Private;
                    break;

                case AsymmetricKeyStrength.RSA_3072:
                    KeyStrength = 3072;
                    PublicKeyIndex = LockBoxEntityAsymmetricKey.RSA_3072_Public;
                    PrivateKeyIndex = LockBoxEntityAsymmetricKey.RSA_3072_Private;
                    break;

                case AsymmetricKeyStrength.RSA_15360:
                    KeyStrength = 15360;
                    PublicKeyIndex = LockBoxEntityAsymmetricKey.RSA_15360_Public;
                    PrivateKeyIndex = LockBoxEntityAsymmetricKey.RSA_15360_Private;
                    break;

                default:
                    throw new NotImplementedException("Asymmetric key strength not implemented");
            }
            RSA.KeySize = KeyStrength;

            // Save the public and private key, protect the private key in memory
            AsymmetricPublicKeyMaterial[PublicKeyIndex] = RSA.ExportCspBlob(false);
            AsymmetricPrivateKeyMaterial[PrivateKeyIndex] = RSA.ExportCspBlob(true);
        }


        public RSACryptoServiceProvider GetRSACryptoServiceProvider(LockBoxEntityAsymmetricKey ThisKey)
        {
            if (IsProtected)
            {
                // We can only return a RSA provider after the user has unprotected
                // the object
                return (null);
            }

            try
            {
                bool IsPublicKey = true;
                switch (ThisKey)
                {
                    case LockBoxEntityAsymmetricKey.RSA_1024_Public:
                    case LockBoxEntityAsymmetricKey.RSA_2048_Public:
                    case LockBoxEntityAsymmetricKey.RSA_3072_Public:
                    case LockBoxEntityAsymmetricKey.RSA_15360_Public:
                        IsPublicKey = true;
                        break;

                    default:
                        IsPublicKey = false;
                        break;

                }

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                if (IsPublicKey)
                {
                    // User wants a public key
                    RSA.ImportCspBlob(AsymmetricPublicKeyMaterial[ThisKey]);
                }
                else
                {
                    // User wants a private key
                    RSA.ImportCspBlob(AsymmetricPrivateKeyMaterial[ThisKey]);
                }
                return (RSA);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("GetRSACryptoServiceProvider", e.Message, true);
                return (null);
            }
        }


        public void DropPrivateData()
        {
            // Clear out the private material, salt, iterations and protection IV
            // salt and iterations and protection IV ok to share, but why if we don't need to
            this.AsymmetricPrivateKeyMaterial.Clear();
            
            /* Overkill, this is publicly sharable, but we don't via filter's we put on 
             * commands
            this.PasswordSalt = null; 
            this.KeyDerivationIterations = -1;
            this.ProtectionIV = null;
            */

            // Public material is not protected, so set it so
            this.IsProtected = false;
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Drop extended crypto data such as password salt,
        ///     key derivation iterations and protection IV
        /// </summary>
        //---------------------------------------------------------------------
        public void DropExtendedData()
        {
            this.PasswordSalt = null;
            this.KeyDerivationIterations = -1;
            this.ProtectionIV = null;

        }


        public static bool AreEqual(LockBoxEntityCryptoData C1, LockBoxEntityCryptoData C2)
        {
            try
            {
                // Compare the two given objects
                if ((C1 == null) || (C2 == null))
                {
                    throw new Exception("At least one object is null");
                }

                // Compare the public key data
                if (C1.AsymmetricPublicKeyMaterial.Count != C2.AsymmetricPublicKeyMaterial.Count)
                {
                    throw new Exception("Public key material count did not match");
                }
                foreach (KeyValuePair<LockBoxEntityAsymmetricKey, byte[]> kvp in C1.AsymmetricPublicKeyMaterial)
                {
                    // Test the current asymmetric key exists in C2
                    if (!C2.AsymmetricPublicKeyMaterial.ContainsKey(kvp.Key))
                    {
                        throw new Exception("C2 does not contain key for " + kvp.Key.ToString());
                    }

                    // Test that they are the same
                    if (!StreamHelper.ByteBuffersAreEqual(C1.AsymmetricPublicKeyMaterial[kvp.Key], C2.AsymmetricPublicKeyMaterial[kvp.Key]))
                    {
                        throw new Exception("C1 and C2 data is not match for " + kvp.Key.ToString());
                    }
                }

                // Compare the private key data
                if (C1.AsymmetricPrivateKeyMaterial.Count != C2.AsymmetricPrivateKeyMaterial.Count)
                {
                    throw new Exception("Private key material count did not match");
                }
                foreach (KeyValuePair<LockBoxEntityAsymmetricKey, byte[]> kvp in C1.AsymmetricPrivateKeyMaterial)
                {
                    // Test the current asymmetric key exists in C2
                    if (!C2.AsymmetricPrivateKeyMaterial.ContainsKey(kvp.Key))
                    {
                        throw new Exception("C2 does not contain key for " + kvp.Key.ToString());
                    }

                    // Test that they are the same
                    if (!StreamHelper.ByteBuffersAreEqual(C1.AsymmetricPrivateKeyMaterial[kvp.Key], C2.AsymmetricPrivateKeyMaterial[kvp.Key]))
                    {
                        throw new Exception("C1 and C2 data is not match for " + kvp.Key.ToString());
                    }
                }


                // Compare the extended key data
                if (C1.KeyDerivationIterations != C2.KeyDerivationIterations)
                {
                    throw new Exception("Key iterations did not match");
                }
                if (!StreamHelper.ByteBuffersAreEqual(C1.PasswordSalt, C2.PasswordSalt))
                {
                    throw new Exception("Password salts did not match");
                }
                if (!StreamHelper.ByteBuffersAreEqual(C1.ProtectionIV, C2.ProtectionIV))
                {
                    throw new Exception("Protection IVs did not match");
                }
                if (C1.ProtectionKeyStrength != C2.ProtectionKeyStrength)
                {
                    throw new Exception("Protection key strengths did not match");
                }
                if (C1.IsProtected != C2.IsProtected)
                {
                    throw new Exception("Protection flags did not match");
                }


                // Passed all the tests
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("LockBoxEntityCryptoData->AreEqual", e.Message);
                return (false);
            }
        }
    }


}
