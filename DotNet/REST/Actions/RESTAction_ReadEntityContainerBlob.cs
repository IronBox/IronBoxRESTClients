using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using RestSharp;
using System.Diagnostics;
using LockBox.Common;
using LockBox.Common.IO;
using System.IO;
using System.Security.Cryptography;

namespace LockBox
{
    public class RESTAction_ReadEntityContainerBlob : RESTActionBase
    {
        private ProtectedContainerKeyData m_ProtectionKeyMaterial = null;

        public RESTAction_ReadEntityContainerBlob(String BaseUrl, ProtectedContainerKeyData ProtectionKeyData)
            : base(BaseUrl)
        {
            m_ProtectionKeyMaterial = ProtectionKeyData;
        }

        public Stream ReadEntityContainerBlob(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            long ContainerID, String BlobID)
        {
            try
            {
                // Input validation
                if ((ContainerID < 0) || String.IsNullOrEmpty(BlobID))
                {
                    throw new Exception("Input error");
                }

                // First create a blob ID
                // Form the REST request, POST {version}/CreateEntityContainerBlob
                RequestObj.Resource = String.Format("{0}/ReadEntityContainerBlob", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyContainerIDToRestRequest(RequestObj, ContainerID);
                LockBoxRESTHelper.ApplyBlobIDToRestRequest(RequestObj, BlobID);

                // Send the request and get the Blob read data
                RESTBlobReadData BlobReadData = Execute<RESTBlobReadData>(RequestObj);
                if (BlobReadData == null)
                {
                    throw new Exception("Invalid blob read data returned");

                }

                // Create a managed blob to take care of the reading for us
                LockBoxManagedBlob ManagedBlob = new LockBoxManagedBlob(
                    new Uri(BlobReadData.StorageUri),
                    BlobReadData.SharedAccessSignature,
                    (LockBoxStorageType)Enum.Parse(typeof(LockBoxStorageType), BlobReadData.StorageType.ToString()),
                    BlobReadData.ContainerStorageName, BlobID, m_ProtectionKeyMaterial.GetSymmetricAlgorithmObject());
                if (!ManagedBlob.LoadBlobDataIntoTempFileStream())
                {
                    throw new Exception("Unable to read blob data into managed blob object");
                }
                // Done, return the blob ID created for the blob we just wrote
                return (ManagedBlob.BlobDataStream);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->ReadEntityContainerBlob", e.Message);
                LastError = e.Message;
                return (null);
            }

            
        }
    }
}
