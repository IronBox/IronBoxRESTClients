using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using RestSharp;
using System.Diagnostics;
using LockBox.Common;
using LockBox.Common.IO;


namespace LockBox
{
    public class RESTAction_CheckOutEntityContainerBlob : RESTActionBase
    {
        public RESTAction_CheckOutEntityContainerBlob(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public RESTBlobCheckOutData CheckOutEntityContainerBlob(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            long ContainerID, String BlobID)
        {
            try
            {
                // Form the REST request, POST {version}/RemoveEntityContainerBlob
                RequestObj.Resource = String.Format("{0}/CheckOutEntityContainerBlob", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Set the container ID and entity validation information on the REST request
                LockBoxRESTHelper.ApplyContainerIDToRestRequest(RequestObj, ContainerID);
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyBlobIDToRestRequest(RequestObj, BlobID);

                // Execute the REST request
                return (Execute<RESTBlobCheckOutData>(RequestObj));
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->CheckOutEntityContainerBlob", e.Message);
                LastError = e.Message;
                return (null);
            }
        }
    }
}
