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
    public class RESTAction_RemoveEntityContainer : RESTActionBase
    {
        public RESTAction_RemoveEntityContainer(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public bool RemoveEntityContainer(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            long ContainerID)
        {
            try
            {
                // Form the REST request, POST {version}/CreateEntityContainer
                RequestObj.Resource = String.Format("{0}/RemoveEntityContainer", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Set the container ID and entity validation information on the REST request
                LockBoxRESTHelper.ApplyContainerIDToRestRequest(RequestObj, ContainerID);
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);

                // Execute the REST request
                return (Boolean.Parse(Execute(RequestObj).Content));               
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->RemoveEntityContainer", e.Message);
                LastError = e.Message;
                return (false);
            }
        }
    }
}
