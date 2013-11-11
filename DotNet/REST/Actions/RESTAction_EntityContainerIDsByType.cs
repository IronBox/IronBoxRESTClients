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
    public class RESTAction_EntityContainersIDByType : RESTActionBase
    {
        public RESTAction_EntityContainersIDByType(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public long[] GetEntityContainersIDByType(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password, 
            LockBoxContainerProtectionMode[] ContainerTypes)
        {
            try
            {
                // Input validation
                if ((ContainerTypes == null) || (ContainerTypes.Length == 0))
                {
                    throw new Exception("No container types specified, nop");
                }

                RequestObj.Resource = String.Format("{0}/EntityContainerIDsByType", APIVersion.ToString());
                RequestObj.Method = Method.POST;
                RequestObj.RequestFormat = DataFormat.Json;

                // Create a list of ints based on the container types given
                List<String> ListCSV = new List<string>();
                foreach (LockBoxContainerProtectionMode P in ContainerTypes)
                {
                    ListCSV.Add(((int)P).ToString());
                }

                // Add the entity validation and list csv
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyListCSVToRestRequest(RequestObj, ListCSV.ToArray());

                // Read the response from the REST service, remove the surrounding [ ] so we get CSV list 
                // of container IDs i.e. [123,123,123]
                String RawResult = NormalizeResponseString(Execute(RequestObj).Content);
                //Debug.WriteLine("Content was: " + RawResult);

                // Parse the CSV of IDs into an array if the raw result was not 
                // empty
                List<long> ResultsIDs = new List<long>();
                if (!String.IsNullOrEmpty(RawResult))
                {

                    string[] Tokens = RawResult.Split(new char[] { ',' });
                    foreach (String s in Tokens)
                    {
                        Debug.WriteLine(String.Format("Parsing [{0}]", s));
                        ResultsIDs.Add(Int64.Parse(s));
                    }
                }

                // Return the results
                return (ResultsIDs.ToArray());
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->GetEntityPrivateKeyData", e.Message);
                LastError = e.Message;
                return (null);
            }
        }
    }
}
