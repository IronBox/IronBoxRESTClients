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
    public class RESTAction_GetContextSetting : RESTActionBase
    {


        public RESTAction_GetContextSetting(String BaseUrl)
            : base(BaseUrl)
        {
            
        }

        public String GetContextSetting(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password,
            String Context, ContextSetting SettingToFetch)
        {
            try
            {
                // Input validation
                if (String.IsNullOrEmpty(Context))
                {
                    throw new Exception("Input error");
                }

                
                // We can save ourselves a round-trip by checking if the setting is
                // authorized or not so check here, even if someone were to be able
                // to bypass this, we still check this on the server side as well
                if (!LockBoxRESTHelper.IsAuthorizedContextSetting(SettingToFetch))
                {
                    throw new Exception("Not authorized context setting, client-side");
                }
                

                // First create a blob ID
                // Form the REST request, POST {version}/CreateEntityContainerBlob
                RequestObj.Resource = String.Format("{0}/GetContextSetting", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyContextToRestRequest(RequestObj, Context);
                LockBoxRESTHelper.ApplyContextSettingToRestRequest(RequestObj, SettingToFetch);
                
                // Send the request and get the result
                String RawResult = Execute(RequestObj).Content;
                //Debug.WriteLine("raw result = " + RawResult);
                return (NormalizeResponseString(RawResult));
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->GetContextSetting", e.Message);
                LastError = e.Message;
                return (null);
            }
        }
    }
}
