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
    public class RESTAction_GetDataLeakChecks : RESTActionBase
    {


        public RESTAction_GetDataLeakChecks(String BaseUrl)
            : base(BaseUrl)
        {
            
        }

        public RESTDataLeakCheck[] GetDataLeakChecks(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            String Context)
        {
            try
            {
                // Input validation
                if (String.IsNullOrEmpty(Context))
                {
                    throw new Exception("Input error");
                }

                // First create a blob ID
                // Form the REST request, POST {version}/CreateEntityContainerBlob
                RequestObj.Resource = String.Format("{0}/GetDataLeakChecks", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyContextToRestRequest(RequestObj, Context);
                
                // Send the request and get the result
                RESTDataLeakCheckList ResultList = Execute<RESTDataLeakCheckList>(RequestObj);
                return (ResultList.DataLeakCheckArray.ToArray());
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->GetDataLeakChecks", e.Message);
                LastError = e.Message;
                return (null);
            }

            
        }
    }
}
