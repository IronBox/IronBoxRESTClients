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
    public class RESTAction_EntityPrivateKeyData : RESTActionBase
    {
        public RESTAction_EntityPrivateKeyData(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public byte[] GetEntityPrivateKeyData(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, String Password, LockBoxEntityAsymmetricKey PrivateKey)
        {
            try
            {
                if (!LockBoxRESTHelper.IsValidPrivateKeyIndex(PrivateKey))
                {
                    throw new ArgumentException("Invalid asymmetric key specified, skipping " + PrivateKey.ToString());
                }
                
                RequestObj.Resource = String.Format("{0}/EntityPrivateKeyMaterial", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation and private key index
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyPrivateKeyIndexToRestRequest(RequestObj, PrivateKey);

                // Read the response from the REST service
                String Base64Result = Execute(RequestObj).Content;
                if (String.IsNullOrEmpty(Base64Result))
                {
                    throw new Exception("No response returned");
                }
                Debug.WriteLine("Response: " + Base64Result);

                // Not sure why sometimes we get double quotes in the response, but trim these
                Base64Result = NormalizeResponseString(Base64Result);
                return (StreamHelper.GetByteBufferFromBase64EncodedString(Base64Result));
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
