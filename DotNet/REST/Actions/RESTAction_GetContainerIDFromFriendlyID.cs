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
    public class RESTAction_GetContainerIDFromFriendlyID : RESTActionBase
    {


        public RESTAction_GetContainerIDFromFriendlyID(String BaseUrl)
            : base(BaseUrl)
        {
            
        }

        public long GetContainerIDFromFriendlyID(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            String FriendlyID)
        {
            try
            {
                // Input validation
                if (String.IsNullOrEmpty(FriendlyID))
                {
                    throw new Exception("Input error");
                }

                // First create a blob ID
                // Form the REST request, POST {version}/CreateEntityContainerBlob
                RequestObj.Resource = String.Format("{0}/GetContainerIDFromFriendlyID", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyContainerFriendlyIDToRestRequest(RequestObj, FriendlyID);
                
                // Send the request and get the result
                //Debug.WriteLine(Execute(RequestObj).Content);
                long ContainerID = Int64.Parse(Execute(RequestObj).Content);
                return (ContainerID);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->GetContainerIDFromFriendlyID", e.Message);
                LastError = e.Message;
                return (-1);
            }

            
        }
    }
}
