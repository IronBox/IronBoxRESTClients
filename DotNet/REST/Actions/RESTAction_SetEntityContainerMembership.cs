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
    public class RESTAction_SetEntityContainerMembership : RESTActionBase
    {
        public RESTAction_SetEntityContainerMembership(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public bool SetEntityContainerMembership(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password, long ContainerID,
            String Entity2, LockBoxEntityIDType Entity2Type, LockBoxContainerRightsCollection ApplyTheseRights)
        {
            try
            {
                RequestObj.Resource = String.Format("{0}/SetEntityContainerMembership", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation and private key index
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyEntity2ToRestRequest(RequestObj, Entity2);
                LockBoxRESTHelper.ApplyEntityType2ToRestRequest(RequestObj, Entity2Type);
                LockBoxRESTHelper.ApplyContainerIDToRestRequest(RequestObj, ContainerID);
                LockBoxRESTHelper.ApplyContainerRightsToRestRequest(RequestObj, ApplyTheseRights);

                // Done, return response code
                return (Boolean.Parse(Execute(RequestObj).Content));
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->SetEntityContainerMembership", e.Message);
                LastError = e.Message;
                return (false);
            }
        }
    }
}
