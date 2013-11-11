using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RestSharp;
using System.Diagnostics;
using LockBox.Common;
using LockBox.Common.IO;
using System.Net.Http.Formatting;
using LockBox.Common.Security.Cryptography;

namespace LockBox
{
    public partial class LockBoxRESTHelper
    {
        // Form data container data keys
        public static String FormData_ContainerIDKey = "ContainerID";
        public static String FormData_ContainerFriendlyID = "ContainerFriendlyID";
        public static String FormData_ContainerOwnerEntityIDKey = "ContainerOwnerEntityID";
        public static String FormData_ContainerStorageEndpointIDKey = "ContainerStorageEndpointID";
        public static String FormData_ContainerStorageNameKey = "ContainerStorageName";        
        public static String FormData_ContainerProtectionModeKey = "ContainerProtectionMode";
        public static String FormData_ContainerExpirationUtcKey = "ContainerExpirationUtc";
        public static String FormData_ContainerAvailableUtcKey = "ContainerAvailableUtc";
        public static String FormData_ContainerSymmetricProtectionKey = "ContainerSymmetricProtection";
        public static String FormData_ContainerSymmetricIVKey = "ContainerSymmetricIV";
        public static String FormData_ContainerAsymmetricProtectionKey = "ContainerAsymmetricProtection";
        public static String FormData_ContainerNameKey = "ContainerName";
        public static String FormData_ContainerDescriptioKey = "ContainerDescription";
        public static String FormData_ContainerFileNameSaltKey = "ContainerFileNameSalt";
        public static String FormData_ContainerEnabledKey = "ContainerEnabled";

        public static String FormData_ContainerRightsCSV = "ContainerRightsCSV";


        public static void ApplyContainerRightsToRestRequest(RestRequest ThisRequest, LockBoxContainerRightsCollection Rights)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_ContainerRightsCSV, Rights.GetDBStorageString());
            }
        }

        public static LockBoxContainerRightsCollection GetContainerRightsFromFormData(FormDataCollection FormData)
        {
            LockBoxContainerRightsCollection Result = new LockBoxContainerRightsCollection();
            Result.LoadDBStorableString(FormData.Get(FormData_ContainerRightsCSV));
            return (Result);
        }


        public static void ApplyMailClientSFTContainerConfigToRestRequest(RestRequest ThisRequest, RESTMailClientContainerConfig ContainerConfig)
        {
            if ((ThisRequest != null) && (ContainerConfig != null))
            {
                ContainerConfig.LoadIntoRestRequest(ThisRequest);
            }
        }

        public static RESTMailClientContainerConfig GetMailClientSFTContainerConfigFromFormData(FormDataCollection FormData)
        {
            try
            {
                RESTMailClientContainerConfig Result = new RESTMailClientContainerConfig();
                if (!Result.LoadFromFormDataCollection(FormData))
                {
                    throw new Exception("Unable to load REST mail client container config");
                }
                return (Result);
            }
            catch (Exception e)
            {
                return (null);
            }
        }


        public static void ApplyContainerConfigToRestRequest(RestRequest ThisRequest, EntityContainer ContainerConfig)
        {
            if ((ThisRequest != null) && (ContainerConfig != null))
            {
                ThisRequest.AddParameter(FormData_ContainerIDKey, ContainerConfig.ContainerID);
                ThisRequest.AddParameter(FormData_ContainerFriendlyID, ContainerConfig.FriendlyID);
                ThisRequest.AddParameter(FormData_ContainerOwnerEntityIDKey, ContainerConfig.OwnerEntityID);
                ThisRequest.AddParameter(FormData_ContainerStorageEndpointIDKey, ContainerConfig.StorageEndpointID);
                ThisRequest.AddParameter(FormData_ContainerStorageNameKey, ContainerConfig.ContainerStorageName);
                ThisRequest.AddParameter(FormData_ContainerProtectionModeKey, (int)ContainerConfig.ProtectionMode);
                ThisRequest.AddParameter(FormData_ContainerExpirationUtcKey, ContainerConfig.ExpirationUtc.ToString());
                ThisRequest.AddParameter(FormData_ContainerAvailableUtcKey, ContainerConfig.AvailableUtc.ToString());
                ThisRequest.AddParameter(FormData_ContainerSymmetricProtectionKey, (int)ContainerConfig.SymmetricProtection);
                ThisRequest.AddParameter(FormData_ContainerSymmetricIVKey, ContainerConfig.SymmetricIV == null ? String.Empty : StringHelper.GetBase64EncodedStringFromByteBuffer(ContainerConfig.SymmetricIV));
                ThisRequest.AddParameter(FormData_ContainerAsymmetricProtectionKey, (int)ContainerConfig.AsymmetricProtection);
                ThisRequest.AddParameter(FormData_ContainerNameKey, String.IsNullOrEmpty(ContainerConfig.Name) ? String.Empty : ContainerConfig.Name);
                ThisRequest.AddParameter(FormData_ContainerDescriptioKey, String.IsNullOrEmpty(ContainerConfig.Description) ? String.Empty : ContainerConfig.Description);
                ThisRequest.AddParameter(FormData_ContainerFileNameSaltKey, String.IsNullOrEmpty(ContainerConfig.FileNameSalt) ? String.Empty : ContainerConfig.FileNameSalt);
                //ThisRequest.AddParameter(FormData_ContainerEnabledKey, ContainerConfig.Enabled);
                SetContainerEnabledOnRESTRequest(ThisRequest, ContainerConfig.Enabled);
            }
        }


        public static EntityContainer GetContainerConfigFromFormData(FormDataCollection FormData)
        {
            try
            {
                EntityContainer Result = new EntityContainer();
                Result.ContainerID = GetContainerIDFromFormData(FormData);
                Result.FriendlyID = FormData.Get(FormData_ContainerFriendlyID);
                Result.OwnerEntityID = Int64.Parse(FormData.Get(FormData_ContainerOwnerEntityIDKey));
                Result.StorageEndpointID = Int64.Parse(FormData.Get(FormData_ContainerStorageEndpointIDKey));
                Result.ContainerStorageName = FormData.Get(FormData_ContainerStorageNameKey);
                Result.ProtectionMode = (LockBoxContainerProtectionMode)Enum.Parse(typeof(LockBoxContainerProtectionMode), FormData.Get(FormData_ContainerProtectionModeKey));
                Result.ExpirationUtc = DateTime.Parse(FormData.Get(FormData_ContainerExpirationUtcKey));
                Result.AvailableUtc = DateTime.Parse(FormData.Get(FormData_ContainerAvailableUtcKey));
                Result.SymmetricProtection = (SymmetricKeyStrength)Enum.Parse(typeof(SymmetricKeyStrength), FormData.Get(FormData_ContainerSymmetricProtectionKey));
                Result.SymmetricIV = String.IsNullOrEmpty(FormData.Get(FormData_ContainerSymmetricIVKey)) ? null : StreamHelper.GetByteBufferFromBase64EncodedString(FormData.Get(FormData_ContainerSymmetricIVKey));
                Result.AsymmetricProtection = (AsymmetricKeyStrength)Enum.Parse(typeof(AsymmetricKeyStrength), FormData.Get(FormData_ContainerAsymmetricProtectionKey));
                Result.Name = FormData.Get(FormData_ContainerNameKey);
                Result.Description = FormData.Get(FormData_ContainerDescriptioKey);
                Result.FileNameSalt = FormData.Get(FormData_ContainerFileNameSaltKey);
                Result.Enabled = GetContainerEnabledFromFormData(FormData);
                return (Result);
            }
            catch (Exception)
            {
                return (null);
            }
        }


        public static void ApplyContainerNameToRestRequest(RestRequest ThisRequest, String ContainerName)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_ContainerNameKey, ContainerName);
            }
        }

        public static String GetContainerNameFromFormData(FormDataCollection FormData)
        {
            return (FormData.Get(FormData_ContainerNameKey));
        }
        

        public static void ApplyContainerIDToRestRequest(RestRequest ThisRequest, long ContainerID)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_ContainerIDKey, ContainerID);
            }
        }

        
        public static long GetContainerIDFromFormData(FormDataCollection FormData)
        {

            return (Int64.Parse(FormData.Get(FormData_ContainerIDKey)));

        }

        public static void SetContainerEnabledOnRESTRequest(RestRequest ThisRequest, bool IsEnabled)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_ContainerEnabledKey, IsEnabled);
            }
        }

        public static bool GetContainerEnabledFromFormData(FormDataCollection FormData)
        {
            return (Boolean.Parse(FormData.Get(FormData_ContainerEnabledKey)));
        }


        public static void ApplyContainerFriendlyIDToRestRequest(RestRequest ThisRequest, String ContainerFriendlyID)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_ContainerFriendlyID, ContainerFriendlyID);
            }
        }

        public static String GetContainerFriendlyIDFromFormData(FormDataCollection FormData)
        {
            return (FormData.Get(FormData_ContainerFriendlyID));
        }
        
    }

    
}
