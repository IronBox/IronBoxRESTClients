using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common;
using LockBox.Common.IO;
using LockBox.Common.Security.Cryptography;
using System.Security.Cryptography;
using System.IO;
using LockBox.Common.IO.Compression;
using System.IO.Compression;
using LockBox;

namespace LockBox
{
    public class LockBoxProtectData
    {

        public static int ProtectionBlockSize = 2048;

        public static bool SymmetricDecryptStream(Stream InputData, Stream OutputStream, SymmetricKeyStrength EncryptionStrength,
            byte[] Key, byte[] IV, CompressionAlgorithm CompressionMethod)
        {
            try
            {
                Stream WorkingWriterStream = OutputStream;

                // We need to decrypt first, then decompress, so our stack
                // looks like the following top down:
                //
                //  Decryption
                //  Decompression
                //  Underlying

                // Decompression stream
                if (CompressionMethod != CompressionAlgorithm.None)
                {
                    WorkingWriterStream = CompressionHelper.CreateCompressionStream(CompressionMethod, WorkingWriterStream, CompressionMode.Decompress);
                }

                // Decryption stream
                if (EncryptionStrength != SymmetricKeyStrength.None)
                {
                    LockBoxDebugHelper.Debug_Log("DecryptAndDecompressStream", "Creating decryption stream, " + EncryptionStrength.ToString(), false);
                    SymmetricAlgorithm sa = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(EncryptionStrength);
                    sa.Key = Key;
                    sa.IV = IV;
                    WorkingWriterStream = new CryptoStream(WorkingWriterStream, sa.CreateDecryptor(), CryptoStreamMode.Write);
                }

                //--------------------------------------------------------------
                // Start writing the encrypted/compressed blob into our processing
                // stream
                //--------------------------------------------------------------
                BinaryWriter BWriter = new BinaryWriter(WorkingWriterStream);
                BinaryReader BReader = new BinaryReader(InputData);
                byte[] Buffer = new byte[ProtectionBlockSize];
                int NumBytesRead = 0;
                while ((NumBytesRead = BReader.Read(Buffer, 0, Buffer.Length)) != 0)
                {
                    BWriter.Write(Buffer, 0, Buffer.Length);
                }

                // Leave the output stream open
                // No errors
                return (true);

            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("DecryptAndDecompressStream", "Error: " + e.Message, true);
                return (false);
            }

        }

        //-------------------------------------------------------------
        /// <summary>
        ///     
        /// </summary>
        /// <param name="InputData"></param>
        /// <param name="OutputStream"></param>
        /// <param name="EncryptionStrength"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <param name="CompressionMethod"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Output stream will be left open
        /// </remarks>
        //-------------------------------------------------------------
        public static bool SymmetricEncryptStream(Stream InputData, Stream OutputStream, SymmetricKeyStrength EncryptionStrength,
            byte[] Key, byte[] IV, CompressionAlgorithm CompressionMethod)
        {
            try
            {
                Stream WorkingWriterStream = OutputStream;
                //-------------------------------------------------------------
                // We need to compress first and then encrypt, but we stack the streams in the opposite, 
                // so top down:
                //      Compression
                //      Encryption
                //      Underlying
                //-------------------------------------------------------------
                // Create encryption stream (if any)
                if (EncryptionStrength != SymmetricKeyStrength.None)
                {
                    LockBoxDebugHelper.Debug_Log("CompressAndEncryptStream", "Creating encryption stream, " + EncryptionStrength.ToString(), false);
                    SymmetricAlgorithm sa = SymmetricEncryptionHelper.GetSymmetricAlgorithmObject(EncryptionStrength);
                    sa.Key = Key;
                    sa.IV = IV;
                    WorkingWriterStream = new CryptoStream(WorkingWriterStream, sa.CreateEncryptor(), CryptoStreamMode.Write);
                }

                // Create compression stream (if any)
                if (CompressionMethod != CompressionAlgorithm.None)
                {
                    LockBoxDebugHelper.Debug_Log("CompressAndEncryptStream", "Creating compression stream, " + CompressionMethod.ToString(), false);
                    WorkingWriterStream = CompressionHelper.CreateCompressionStream(CompressionMethod, WorkingWriterStream, CompressionMode.Compress);
                }

                //--------------------------------------------------------------
                // Start writing data from our BlobData into our working 
                // stream
                //--------------------------------------------------------------
                BinaryWriter BWriter = new BinaryWriter(WorkingWriterStream);
                BinaryReader BReader = new BinaryReader(InputData);
                byte[] Buffer = new byte[ProtectionBlockSize];
                int NumBytesRead = 0;
                while ((NumBytesRead = BReader.Read(Buffer, 0, Buffer.Length)) != 0)
                {
                    BWriter.Write(Buffer, 0, Buffer.Length);
                }

                // Leave the output stream open
                // No errors
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("CompressAndEncryptStream", "Error: " + e.Message, true);
                return (false);
            }

        }


        public static bool SecureDeleteFile(String FilePath)
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    throw new Exception("The given filepath does not exist");
                }

                // Create a random encryption provider and encrypt the contents
                // of the file and throw away the key and IV
                AesCryptoServiceProvider Aes = new AesCryptoServiceProvider();
                Aes.KeySize = 128;

                // TODO: Finish this implementation, if it's  alarge file then only do 1 pass
                // otherwise do n random passes
                // Create random number between 2 and 5
                CryptoRandom CRand = new CryptoRandom();
                CRand.Next(2, 10);
                

                // Delete the encrypted file
                File.Delete(FilePath);
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("SecureDeleteFile", "Unable to securely delete file path: " + e.Message, true);
                return (false);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Converts a given password and salt into a 
        /// </summary>
        /// <param name="Password"></param>
        /// <param name="Salt"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static String ProtectPassword(String Password, String Salt)
        {
            SHA512CryptoServiceProvider sha = new SHA512CryptoServiceProvider();
            return (StringHelper.GetBase64EncodedStringFromByteBuffer(sha.ComputeHash(StreamHelper.GetByteBufferFromString(Password + Salt))));            
        }

        
        /* Moved into KeyDerivationHelper in LockBox.common.security.cryptography
        //---------------------------------------------------------------------
        /// <summary>
        ///     Derives a symmetric encryption key based on a given symmetric 
        ///     key strength, number of iterations, a salt and password
        /// </summary>
        /// <param name="ProtectionKeyStrength"></param>
        /// <param name="KeyDerivationIterations"></param>
        /// <param name="Salt"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static byte[] DeriveSymmetricKey(SymmetricKeyStrength ProtectionKeyStrength, 
            int KeyDerivationIterations, byte[] Salt, String Password)
        {
            // Derive the encryption key and encrypt the private key data
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(Password, Salt, KeyDerivationIterations);
            int ProtectionKeySize = SymmetricEncryptionHelper.GetSymmetricKeySizeBits(ProtectionKeyStrength);
            byte[] ProtectionKey = pbkdf2.GetBytes(ProtectionKeySize / 8);
            return (ProtectionKey);
        }
        */

        

        
    }
}
