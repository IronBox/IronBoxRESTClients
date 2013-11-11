using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common;
using LockBox.Common.Security.Cryptography;
using System.Security.Cryptography;
using LockBox.Common.Data;
using LockBox.Common.IO;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Data.SqlClient;
using LockBox.Common.Data.Sql;

namespace LockBox
{
    public class LockBoxContainerHelper
    {
        

        

        


        public static void Debug_ListContainerNames(ILockBoxStorage StorageObject)
        {
            Debug.WriteLine("+ === STARTING CONTAINER NAMES LISTING");
            foreach (String s in StorageObject.GetContainerNames())
            {
                Debug.WriteLine("\t" + s);
            }
            Debug.WriteLine("+ === ENDING CONTAINER NAMES LISTING");
        }

        public static void Debug_ListContainerContents(ILockBoxStorage StorageObject, String ContainerStorageName)
        {
            Debug.WriteLine("+ === STARTING CONTAINER DUMP: " + ContainerStorageName);
            foreach (String s in StorageObject.GetBlobNames(ContainerStorageName))
            {
                Debug.WriteLine("\t" + s);
            }
            Debug.WriteLine("+ === ENDING CONTAINER DUMP: " + ContainerStorageName);
        }

        public static bool ContainersAreEqual(EntityContainer A, EntityContainer B)
        {
            if ((A == null) && (B == null))
            {
                return (true);
            }

            if ((A == null) || (B == null))
            {
                throw new ArgumentNullException("One of the parameters was null, but not both");
            }

            // Check Asymm protection
            if (A.AsymmetricProtection != B.AsymmetricProtection)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "Asymmmetric protection did not match", false);
                return (false);
            }

            // Available Utc
            //if (DateTime.Compare(A.AvailableUtc, B.AvailableUtc) != 0)
            if (A.AvailableUtc.ToString() != B.AvailableUtc.ToString())
            {
                
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", String.Format("AvailableUtc protection did not match [{0}] [{1}]",A.AvailableUtc.ToString(),B.AvailableUtc.ToString()), false);
                return (false);
            }

            // Container ID
            if (A.ContainerID != B.ContainerID)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "Container IDs did not match", false);
                return (false);
            }

            // Storage container name
            if (A.ContainerStorageName != B.ContainerStorageName)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "Container storage name did not match", false);
                return (false);
            }

            // Description
            if (A.Description != B.Description)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "Description did not match", false);
                return (false);
            }

            // Enabled
            if (A.Enabled != B.Enabled)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "Enabled did not match", false);
                return (false);
            }

            // Expiration Utc
            //if (DateTime.Compare(A.ExpirationUtc, B.ExpirationUtc) != 0)
            if (A.ExpirationUtc.ToString() != B.ExpirationUtc.ToString())
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "ExpirationUtc did not match", false);
                return (false);
            }

            // FileNameSalt
            if (A.FileNameSalt != B.FileNameSalt)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "FileNameSalt did not match", false);
                return (false);
            }

            // FriendlyID
            if (A.FriendlyID != B.FriendlyID)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "FriendlyID did not match", false);
                return (false);
            }

            // Name
            if (A.Name != B.Name)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "Name did not match", false);
                return (false);
            }

            // OwnerEntityID
            if (A.OwnerEntityID != B.OwnerEntityID)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "OwnerEntityID did not match", false);
                return (false);
            }

            // ProtectionMode
            if (A.ProtectionMode != B.ProtectionMode)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "ProtectionMode did not match", false);
                return (false);
            }

            // StorageEndpointID
            if (A.StorageEndpointID != B.StorageEndpointID)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "StorageEndpointID did not match", false);
                return (false);
            }

            // SymmetricIV
            if (!StreamHelper.ByteBuffersAreEqual(A.SymmetricIV, B.SymmetricIV, true))
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "SymmetricIV did not match", false);
                return (false);
            }

            // SymmetricProtection
            if (A.SymmetricProtection != B.SymmetricProtection)
            {
                LockBoxDebugHelper.Debug_Log("ContainersAreEqual", "SymmetricProtection did not match", false);
                return (false);
            }

            // Done, passed all checks
            return (true);

        }

        /*
        public static bool EncryptContainerMetaDataValues(GenericMetaData MetaData, SymmetricAlgorithm SAProvider)
        {
            try
            {
                if (MetaData == null)
                {
                    throw new ArgumentNullException("Meta data object was null");
                }
                if (SAProvider == null)
                {
                    throw new ArgumentNullException("Symmetric algorithm provider was null");
                }

                // Iterate through all the string dictionary data and encrypt the values
                foreach (LockBoxContainerMetaDataKey Key in MetaData.MetaData.Keys)
                {
                    String ValueToEncrypt = MetaData.MetaData[Key];
                    if (DataValidator.IsStringNullOrEmpty(ValueToEncrypt))
                    {
                        // If empty nothing to do, so just insert an empty string 
                        // and skip right to the next key
                        MetaData.MetaData[Key] = String.Empty;
                        continue;
                    }

                    // Do encryption
                    MetaData.MetaData[Key] = SymmetricEncryptionHelper.EncryptStringToBase64Value(SAProvider, ValueToEncrypt);
                }

                // Done, no errors
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("EncryptContainerMetaDataValues", e.Message, true);
                return (false);
            }
        }


        public static bool DecryptContainerMetaDataValues(GenericMetaData MetaData, SymmetricAlgorithm SAProvider)
        {
            try
            {
                if (MetaData == null)
                {
                    throw new ArgumentNullException("Meta data object was null");
                }
                if (SAProvider == null)
                {
                    throw new ArgumentNullException("Symmetric algorithm provider was null");
                }

                // Iterate through all the string dictionary data and encrypt the values
                foreach (LockBoxContainerMetaDataKey Key in MetaData.MetaData.Keys)
                {
                    String ValueToDecrypt = MetaData.MetaData[Key];
                    if (DataValidator.IsStringNullOrEmpty(ValueToDecrypt))
                    {
                        // Nop if the value is null or empty
                        continue;
                    }

                    // Do encryption
                    MetaData.MetaData[Key] = SymmetricEncryptionHelper.DecryptBase64StringValue(SAProvider, ValueToDecrypt);
                }

                // Done, no errors
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("DecryptContainerMetaDataValues", e.Message, true);
                return (false);
            }
        }
        */
    }
}
