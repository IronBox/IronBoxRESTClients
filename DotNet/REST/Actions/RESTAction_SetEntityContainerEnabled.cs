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
    public class RESTAction_SetEntityContainerEnabled : RESTActionBase
    {
        public RESTAction_SetEntityContainerEnabled(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public bool SetEntityContainerEnabled(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            long ContainerID, bool IsEnabled)
        {
            
            try
            {
                // Form the REST request, POST {version}/CreateEntityContainer
                RequestObj.Resource = String.Format("{0}/SetEntityContainerEnabled", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Set the parameters to enable or disable container
                LockBoxRESTHelper.ApplyContainerIDToRestRequest(RequestObj, ContainerID);
                LockBoxRESTHelper.SetContainerEnabledOnRESTRequest(RequestObj, IsEnabled);

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);

                
                return (Boolean.Parse(Execute(RequestObj).Content));               
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->SetEntityContainerEnabled", e.Message);
                LastError = e.Message;
                return (false);
            }
        }
    }
}
