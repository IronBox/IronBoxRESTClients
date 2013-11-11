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
    public class RESTAction_CheckEntitiesContextMembershipInRole : RESTActionBase
    {


        public RESTAction_CheckEntitiesContextMembershipInRole(String BaseUrl)
            : base(BaseUrl)
        {
            
        }

        public String[] CheckEntitiesContextMembership(LockBoxAPIVersion APIVersion, String Context, String EntitiesCSV, LockBoxEntityIDType EntitiesCSVType, ContextRole ThisRole)
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
                RequestObj.Resource = String.Format("{0}/CheckEntitiesContextMembershipInRole", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyContextToRestRequest(RequestObj, Context);
                LockBoxRESTHelper.ApplyEntitiesCSVToRestRequest(RequestObj, EntitiesCSV, EntitiesCSVType);
                LockBoxRESTHelper.ApplyContextRoleToRestRequest(RequestObj, ThisRole);
                
                // Send the request and get the result, and parse into an array of bools
                String ResponseCSV = NormalizeResponseString(Execute(RequestObj).Content);

                // Parse the response into a string array
                String[] Results = null;
                if (!StringHelper.ParseCSVString(ResponseCSV, out Results))
                {
                    throw new Exception("Results were unparsable");
                }

                // Done, return the results as an array of true/false
                return (Results);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->CheckEntitiesContextMembershipInRole", e.Message);
                LastError = e.Message;
                return (null);
            }

            
        }
    }
}
