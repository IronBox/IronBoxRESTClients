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
    public class RESTAction_CreateEntityOneTimeToken : RESTActionBase
    {
        public RESTAction_CreateEntityOneTimeToken(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public RESTEntityOneTimeTokenData CreateEntityOneTimeToken(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password)
        {
            try
            {
                // Form the REST request, POST {version}/CreateEntityContainer
                RequestObj.Resource = String.Format("{0}/CreateEntityOneTimeToken", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);

                // Execute the REST request
                return (Execute<RESTEntityOneTimeTokenData>(RequestObj));             
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->CreateEntityOneTimeToken", e.Message);
                LastError = e.Message;
                return (null);
            }
        }
    }
}
