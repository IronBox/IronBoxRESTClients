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
    public class RESTAction_WriteEntityContainerBlob : RESTActionBase
    {
        private ProtectedContainerKeyData m_ProtectionKeyMaterial = null;

        public RESTAction_WriteEntityContainerBlob(String BaseUrl, ProtectedContainerKeyData ProtectionKeyData)
            : base(BaseUrl)
        {
            m_ProtectionKeyMaterial = ProtectionKeyData;
        }

        public String WriteEntityContainerBlob(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            long ContainerID, String BlobName, Stream BlobData)
        {
            try
            {
                // Input validation
                if ((ContainerID < 0) || String.IsNullOrEmpty(BlobName) || (BlobData == null))
                {
                    throw new Exception("Input error");
                }

                // First create a blob ID
                // Form the REST request, POST {version}/CreateEntityContainerBlob
                RequestObj.Resource = String.Format("{0}/CreateEntityContainerBlob", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyContainerIDToRestRequest(RequestObj, ContainerID);
                LockBoxRESTHelper.ApplyBlobNameToRestRequest(RequestObj, BlobName);

                // Send the request and get the Blob ID
                String BlobID = NormalizeResponseString(Execute(RequestObj).Content);
                if (String.IsNullOrEmpty(BlobID))
                {
                    throw new Exception("Unable to create blob ID");
                }

                

                // Create a request to get the SAS signature to upload to the blob
                // This is another REST action
                RESTAction_CheckOutEntityContainerBlob RESTActionCheckOut = new RESTAction_CheckOutEntityContainerBlob(BaseUrl);
                RESTBlobCheckOutData CheckOutData = RESTActionCheckOut.CheckOutEntityContainerBlob(APIVersion, Entity, EntityType, Password, ContainerID, BlobID);
                if ((CheckOutData == null) || String.IsNullOrEmpty(CheckOutData.SharedAccessSignature) || 
                    String.IsNullOrEmpty(CheckOutData.SharedAccessSignatureUri) ||
                    String.IsNullOrEmpty(CheckOutData.CheckInToken)
                    )
                {
                    throw new Exception("Unable to get SAS signature, URI or check in token");
                }
                /*
                Debug.WriteLine("SAS: " + CheckOutData.SharedAccessSignature);
                Debug.WriteLine("URI: " + CheckOutData.StorageUri);
                Debug.WriteLine("CSN: " + CheckOutData.ContainerStorageName);
                Debug.WriteLine("BlobID: " + BlobID);
                */

                // Create a managed blob to do the blob write
                LockBoxManagedBlob ManagedBlob = new LockBoxManagedBlob(
                    new Uri(CheckOutData.StorageUri),
                    CheckOutData.SharedAccessSignature,
                    (LockBoxStorageType)Enum.Parse(typeof(LockBoxStorageType), CheckOutData.StorageType.ToString()),
                    CheckOutData.ContainerStorageName, BlobID, m_ProtectionKeyMaterial.GetSymmetricAlgorithmObject());
                if (!ManagedBlob.WriteBlobData(BlobData))
                {
                    throw new Exception("Unable to write blob data directly to blob");
                }
                
                // Check in the blob
                RESTAction_CheckInEntityContainerBlob RESTActionCheckIn = new RESTAction_CheckInEntityContainerBlob(BaseUrl);
                EntityBlobCheckInData CheckInData = new EntityBlobCheckInData();
                CheckInData.CheckInToken = CheckOutData.CheckInToken;
                CheckInData.SizeBytes = BlobData.Length;
                bool CheckInResult = RESTActionCheckIn.CheckInEntityContainerBlob(APIVersion,
                    Entity, EntityType, Password, ContainerID, BlobID, CheckInData);
                if (!CheckInResult)
                {
                    throw new Exception("Unable to check in blob data");
                }

                // Done, return the blob ID created for the blob we just wrote
                return (BlobID);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->WriteEntityContainerBlob", e.Message);
                LastError = e.Message;
                return (null);
            }

            
        }
    }
}
