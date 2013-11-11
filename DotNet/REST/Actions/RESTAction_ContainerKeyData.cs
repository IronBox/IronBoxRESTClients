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
    public class RESTAction_ContainerKeyData : RESTActionBase
    {
        public RESTAction_ContainerKeyData(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public RESTContainerKeyData GetContainerKeyData(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            long ContainerID)
        {
            try
            {
                // Form the REST request, POST {version}/ContainerKeyData
                RequestObj.Resource = String.Format("{0}/ContainerKeyData", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Set the container ID and entity validation information on the REST request
                LockBoxRESTHelper.ApplyContainerIDToRestRequest(RequestObj, ContainerID);
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);

                // Execute the REST request
                return (Execute<RESTContainerKeyData>(RequestObj));                
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->GetContainerKeyData", e.Message);
                LastError = e.Message;
                return (null);
            }
        }
    }
}
