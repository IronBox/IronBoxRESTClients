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
    public class RESTAction_CreateEntityContainer : RESTActionBase
    {
        public RESTAction_CreateEntityContainer(String BaseUrl)
            : base(BaseUrl)
        {

        }
        
        public long CreateEntityContainer(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            EntityContainer ContainerConfig)
        {
            long Result = -1;
            try
            {
                // Form the REST request, POST {version}/CreateEntityContainer
                RequestObj.Resource = String.Format("{0}/CreateEntityContainer", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);

                // Add the container configuration information
                LockBoxRESTHelper.ApplyContainerConfigToRestRequest(RequestObj, ContainerConfig);

                return (Int64.Parse(Execute(RequestObj).Content));               
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->CreateEntityContainer", e.Message);
                LastError = e.Message;
                Result = -1;
            }

            return (Result);
        }
    }
}
