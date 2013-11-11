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
    public class RESTAction_GetContainerIDsFromName : RESTActionBase
    {


        public RESTAction_GetContainerIDsFromName(String BaseUrl)
            : base(BaseUrl)
        {
            
        }

        public long[] GetContainerIDsFromName(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            String ContainerName)
        {
            try
            {
                // Input validation
                if (String.IsNullOrEmpty(ContainerName))
                {
                    throw new Exception("Input error");
                }

                // First create a blob ID
                // Form the REST request, POST {version}/CreateEntityContainerBlob
                RequestObj.Resource = String.Format("{0}/GetContainerIDsFromName", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyContainerNameToRestRequest(RequestObj, ContainerName);
                
                // Send the request and get the result
                //Debug.WriteLine(Execute(RequestObj).Content);
                String RawResponseCSV = NormalizeResponseString(Execute(RequestObj).Content);
                String[] RawResponseArray = null;
                if (!StringHelper.ParseCSVString(RawResponseCSV, out RawResponseArray))
                {
                    throw new Exception("Unable to parse response CSV");
                }
                List<long> Results = new List<long>();
                foreach (String Token in RawResponseArray)
                {
                    long Temp;
                    if (Int64.TryParse(Token, out Temp))
                    {
                        Results.Add(Temp);
                    }

                }

                // Done, return results
                return (Results.ToArray());
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->GetContainerIDsFromName", e.Message);
                LastError = e.Message;
                return (null);
            }

            
        }
    }
}
