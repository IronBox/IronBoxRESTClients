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
    public class RESTAction_CheckInEntityContainerBlob : RESTActionBase
    {
        public RESTAction_CheckInEntityContainerBlob(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public bool CheckInEntityContainerBlob(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            long ContainerID, String BlobID, EntityBlobCheckInData CheckInData)
        {
            try
            {
                // Form the REST request, POST {version}/CheckInEntityContainerBlob
                RequestObj.Resource = String.Format("{0}/CheckInEntityContainerBlob", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Set the container ID and entity validation information on the REST request
                LockBoxRESTHelper.ApplyContainerIDToRestRequest(RequestObj, ContainerID);
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyBlobIDToRestRequest(RequestObj, BlobID);
                LockBoxRESTHelper.ApplyBlobCheckInDataToRestRequest(RequestObj, CheckInData);

                // Execute the REST request
                return (Boolean.Parse(Execute(RequestObj).Content));
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->CheckInEntityContainerBlob", e.Message);
                LastError = e.Message;
                return (false);
            }
        }
    }
}
