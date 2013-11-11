using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using LockBox.Common.Security.Cryptography;
using LockBox.Common.IO;
using LockBox.Common;

namespace LockBox
{
    public class ContextKeyMaterial
    {
        public int KeyDerivationIterations { set; get; }
        public byte[] ProtectionIV { set; get; }
        public byte[] Salt { set; get; }
        public SymmetricKeyStrength SymmetricStrength { set; get; }
        public AsymmetricKeyStrength AsymmetricStrength { set; get; }
        public byte[] ProtectedPrivateKeyData { set; get; }
        public byte[] PublicKeyData { set; get; }


        public ContextKeyMaterial()
        {
            Reset();
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Create random context key data
        /// </summary>
        //---------------------------------------------------------------------
        public void Reset()
        {
            KeyDerivationIterations = KeyDerivationHelper.GetCryptoRandomKeyIterations();
            Salt = null;
            AsymmetricStrength = AsymmetricKeyStrength.RSA_3072;
            SymmetricStrength = SymmetricKeyStrength.AES256;
            ProtectionIV = null;
            ProtectedPrivateKeyData = null;
            PublicKeyData = null;
        }

        public bool EncryptString(String s, out byte[] EncryptedBytes)
        {
            // Input validation on string 
            if (String.IsNullOrEmpty(s) || (s.Length > 32))
            {
                LockBoxDebugHelper.Debug_Log("ContextKeyMaterial->EncryptString", "Input cannot null, empty or greater than 32 chars long");
                EncryptedBytes = null;
                return (false);
            }
            return (Encrypt(StreamHelper.GetByteBufferFromString(s), out EncryptedBytes));
        }

        public bool Encrypt(byte[] ClearTextBytes, out byte[] EncryptedBytes)
        {
            try
            {
                switch (AsymmetricStrength)
                {
                    case AsymmetricKeyStrength.RSA_1024:
                    case AsymmetricKeyStrength.RSA_2048:
                    case AsymmetricKeyStrength.RSA_3072:
                    case AsymmetricKeyStrength.RSA_15360:
                        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                        RSA.ImportCspBlob(PublicKeyData);
                        EncryptedBytes = AsymmetricEncryptionHelper.RSAEncrypt(ClearTextBytes, RSA, LockBoxEntityCryptoData.UseOAEPPadding);
                        break;

                    default:
                        throw new NotImplementedException("Asymmetric algorithm not implemented");
                }

                // No errors
                return (true);
            }
            catch (Exception e)
            {
                EncryptedBytes = null;
                return (false);
            }
        }

        public bool Decrypt(byte[] EncryptedData, String ContextPassword, out byte[] ClearTextBytes)
        {
            try
            {
                switch (AsymmetricStrength)
                {
                    case AsymmetricKeyStrength.RSA_1024:
                    case AsymmetricKeyStrength.RSA_2048:
                    case AsymmetricKeyStrength.RSA_3072:
                    case AsymmetricKeyStrength.RSA_15360:
                        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                        RSA.ImportCspBlob(GetUnprotectedPrivateKey(ContextPassword));
                        ClearTextBytes = AsymmetricEncryptionHelper.RSADecrypt(EncryptedData, RSA, LockBoxEntityCryptoData.UseOAEPPadding);
                        break;

                    default:
                        throw new NotImplementedException("Asymmetric algorithm not implemented");
                }

                // No errors
                return (true);
            }
            catch (Exception e)
            {
                ClearTextBytes = null;
                return (false);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Creates new context key material
        /// </summary>
        /// <param name="SymmetricProtectionKeyStrength"></param>
        /// <param name="AsymmetricProtectionKeyStrength"></param>
        /// <param name="ContextAdminPassword"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool CreateNewKeyMaterial(
            SymmetricKeyStrength SymmetricProtectionKeyStrength, 
            AsymmetricKeyStrength AsymmetricProtectionKeyStrength, 
            String ContextAdminPassword)
        {
            // Create some basic new key material
            Reset();

            try
            {
                // Input validation
                if (SymmetricProtectionKeyStrength == SymmetricKeyStrength.None)
                {
                    throw new Exception("Invalid symmetric key strength");
                }
                if (String.IsNullOrEmpty(ContextAdminPassword))
                {
                    throw new Exception("Invalid context admin password");
                }

                // Create some new key material
                this.KeyDerivationIterations = KeyDerivationHelper.GetCryptoRandomKeyIterations();
                this.Salt = StreamHelper.GetByteBufferFromString(Guid.NewGuid().ToString());
                this.SymmetricStrength = SymmetricProtectionKeyStrength;

                // Create the symmetric encryptor
                SymmetricAlgorithm SA = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(this.SymmetricStrength);
                SA.Key = KeyDerivationHelper.DeriveSymmetricKey(this.SymmetricStrength, KeyDerivationIterations, Salt, ContextAdminPassword);
                this.ProtectionIV = SA.IV;
                this.AsymmetricStrength = AsymmetricProtectionKeyStrength;
                
                // Create the asymmetric key pair for the context
                byte[] UnprotectedPrivateKey = null;
                AsymmetricAlgorithm AObject = AsymmetricEncryptionHelper.CreateAsymmetricObject(this.AsymmetricStrength);
                switch (AsymmetricStrength)
                {
                    case AsymmetricKeyStrength.RSA_1024:
                    case AsymmetricKeyStrength.RSA_2048:
                    case AsymmetricKeyStrength.RSA_3072:
                    case AsymmetricKeyStrength.RSA_15360:
                        RSACryptoServiceProvider RSA = (RSACryptoServiceProvider)AObject;
                        this.PublicKeyData = RSA.ExportCspBlob(false);
                        UnprotectedPrivateKey = RSA.ExportCspBlob(true);
                        break;

                    default:
                        throw new NotImplementedException("Asymmetric algorithm not implemented");
                }

                // Protect the private key data
                this.ProtectedPrivateKeyData = SymmetricEncryptionHelper.Symmetric_EncryptOrDecrypt(UnprotectedPrivateKey, SA, CryptoMode.Encrypt);

                // Done
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("ContextKeyMaterial->CreateNewKeyMaterial", e.Message);
                return (false);
            }

        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Encrypts the given clear text private key and sets it as the 
        ///     protected private key in this object
        /// </summary>
        /// <param name="ClearTextPrivateKey"></param>
        /// <param name="ContextPasswordToUse"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool SetAndLockPrivateKey(byte[] ClearTextPrivateKey, String ContextPasswordToUse)
        {
            try
            {
                // Create the symmetric decryptor
                SymmetricAlgorithm SA = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(this.SymmetricStrength);
                SA.Key = DeriveProtectionKey(ContextPasswordToUse);
                SA.IV = ProtectionIV;
                ProtectedPrivateKeyData = SymmetricEncryptionHelper.Symmetric_EncryptOrDecrypt(ClearTextPrivateKey, SA, CryptoMode.Encrypt);
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("ContextKeyMaterial->SetPrivateKey", e.Message);
                return (false);
            }
        }
        

        public byte[] GetUnprotectedPrivateKey(String ContextAdminPassword)
        {
            try
            {
                // Create the symmetric decryptor
                SymmetricAlgorithm SA = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(this.SymmetricStrength);
                SA.Key = DeriveProtectionKey(ContextAdminPassword);
                SA.IV = ProtectionIV;
                return (SymmetricEncryptionHelper.Symmetric_EncryptOrDecrypt(ProtectedPrivateKeyData, SA, CryptoMode.Decrypt));
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("ContextKeyMaterial->GetUnprotectedPrivateKey", e.Message);
                return (null);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Derives a protection key based on the context admin password
        ///     given
        /// </summary>
        /// <param name="ContextPassword"></param>
        /// <returns>
        ///     Returns a derived symmetric key using PBKDF2
        /// </returns>
        //---------------------------------------------------------------------
        public byte[] DeriveProtectionKey(String ContextPassword)
        {
            try
            {
                return (KeyDerivationHelper.DeriveSymmetricKey(SymmetricStrength, 
                    KeyDerivationIterations, Salt, ContextPassword));
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("ContextKeyMaterial->DeriveProtectionKey", e.Message);
                return (null);
            }
        }
        
        
        
    }
}
