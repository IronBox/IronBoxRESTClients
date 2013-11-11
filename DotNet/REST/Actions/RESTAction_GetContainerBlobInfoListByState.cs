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
    public class RESTAction_GetContainerBlobInfoListByState : RESTActionBase
    {


        public RESTAction_GetContainerBlobInfoListByState(String BaseUrl)
            : base(BaseUrl)
        {
            
        }

        public RESTBlobInfo[] GetContainerBlobInfoListByState(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            long ContainerID, LockBoxBlobStatusKey State)
        {
            try
            {
                // Input validation
                if (ContainerID < 0)
                {
                    throw new Exception("Input error");
                }

                // First create a blob ID
                // Form the REST request, POST {version}/CreateEntityContainerBlob
                RequestObj.Resource = String.Format("{0}/GetContainerBlobInfoListByState", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyContainerIDToRestRequest(RequestObj, ContainerID);
                LockBoxRESTHelper.ApplyBlobStateToRestRequest(RequestObj, State);
                
                // Send the request and get the result
                //Debug.WriteLine(Execute(RequestObj).Content);
                RESTBlobInfoList Result = Execute<RESTBlobInfoList>(RequestObj);
                if (Result == null)
                {
                    throw new Exception("Was not able to deserialize a valid object");
                }

                //LockBoxDebugHelper.Debug_Log("REST_Action->GetContainerBlobInfoArrayByState", "Number of objects returned was " + Result.BlobInfoArray.Count);

                // Unwrap from blob info container objects
                List<RESTBlobInfo> BlobInfoList = new List<RESTBlobInfo>();
                foreach (RESTBlobInfo bc in Result.BlobInfoArray)
                {
                    //LockBoxDebugHelper.Debug_Log("REST_Action->GetContainerBlobInfoArrayByState", "BlobID=" + bc.BlobID);
                    //LockBoxDebugHelper.Debug_Log("REST_Action->GetContainerBlobInfoArrayByState", "BlobID=" + bc.BlobName);
                    BlobInfoList.Add(bc);
                }

                /*
                foreach (RESTBlobInfo temp in BlobInfoList)
                {
                    LockBoxDebugHelper.Debug_Log("test","Kevin: " + temp.BlobID);
                    LockBoxDebugHelper.Debug_Log("test", "KevinL: " + temp.BlobID);
                }
                 */

                return (BlobInfoList.ToArray());
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->GetContainerBlobInfoArrayByState", e.Message);
                LastError = e.Message;
                return (null);
            }

            
        }
    }
}
