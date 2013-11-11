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
    public class RESTAction_RemoveEntityContainerMembership : RESTActionBase
    {
        public RESTAction_RemoveEntityContainerMembership(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public bool RemoveEntityContainerMembership(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password, long ContainerID,
            String Entity2, LockBoxEntityIDType Entity2Type)
        {
            try
            {
                RequestObj.Resource = String.Format("{0}/RemoveEntityContainerMembership", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation and private key index
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyEntity2ToRestRequest(RequestObj, Entity2);
                LockBoxRESTHelper.ApplyEntityType2ToRestRequest(RequestObj, Entity2Type);
                LockBoxRESTHelper.ApplyContainerIDToRestRequest(RequestObj, ContainerID);

                // Done, return response code
                String Result = Execute(RequestObj).Content;
                //Debug.WriteLine("Result = " + Result);
                return (Boolean.Parse(Result));
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->RemoveEntityContainerMembership", e.Message);
                LastError = e.Message;
                return (false);
            }
        }
    }
}
