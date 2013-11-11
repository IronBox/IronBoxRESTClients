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
    public class RESTAction_WriteContainerMessage : RESTActionBase
    {
        public RESTAction_WriteContainerMessage(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public bool WriteContainerMessage(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            long ContainerID, String Message)
        {
            try
            {
                // Form the REST request, POST {version}/CreateEntityContainer
                RequestObj.Resource = String.Format("{0}/WriteContainerMessage", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Set the container ID and entity validation information on the REST request, apply the message
                LockBoxRESTHelper.ApplyContainerIDToRestRequest(RequestObj, ContainerID);
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyContainerMessageToRestRequest(RequestObj, Message);

                // Execute the REST request
                String Output = Execute(RequestObj).Content;
                //Debug.WriteLine("Response was: " + Output);
                return (Boolean.Parse(Output));               
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->WriteContainerMessage", e.Message);
                LastError = e.Message;
                return (false);
            }
        }
    }
}
