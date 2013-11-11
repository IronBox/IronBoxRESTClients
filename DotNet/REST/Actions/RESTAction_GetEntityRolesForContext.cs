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
    public class RESTAction_GetEntityRolesForContext : RESTActionBase
    {


        public RESTAction_GetEntityRolesForContext(String BaseUrl)
            : base(BaseUrl)
        {
            
        }

        public ContextRole[] GetEntityRolesForContext(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
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
                RequestObj.Resource = String.Format("{0}/GetEntityRolesForContext", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyContextToRestRequest(RequestObj, Context);
                
                // Send the request and get the result
                String RawResult = NormalizeResponseString(Execute(RequestObj).Content);
                List<ContextRole> ResultContextRoles = new List<ContextRole>();
                if (!String.IsNullOrEmpty(RawResult))
                {

                    string[] Tokens = RawResult.Split(new char[] { ',' });
                    foreach (String s in Tokens)
                    {
                        //Debug.WriteLine(String.Format("Parsing [{0}]", s));
                        ResultContextRoles.Add((ContextRole)Enum.Parse(typeof(ContextRole), s));
                    }
                }

                return (ResultContextRoles.ToArray());
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->GetEntityRolesForContext", e.Message);
                LastError = e.Message;
                return (null);
            }

            
        }
    }
}
