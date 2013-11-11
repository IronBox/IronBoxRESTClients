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
    public class RESTAction_EntityPublicKeyData : RESTActionBase
    {
        public RESTAction_EntityPublicKeyData(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public byte[] GetEntityPublicKeyData(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, LockBoxEntityAsymmetricKey PublicKey)
        {
            try
            {
                if (!LockBoxRESTHelper.IsValidPublicKeyIndex(PublicKey))
                {
                    throw new ArgumentException("Invalid asymmetric key specified, skipping " + PublicKey.ToString());
                }

                // Form the REST request
                RequestObj.Resource = String.Format("{0}/EntityPublicKeyMaterial", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                LockBoxRESTHelper.ApplyPublicKeyIndexToRestRequest(RequestObj, PublicKey);
                LockBoxRESTHelper.ApplyEntityTypeToRestRequest(RequestObj, EntityType);
                LockBoxRESTHelper.ApplyEntityToRestRequest(RequestObj, Entity);

                /*
                Debug.WriteLine(RequestObj.Resource);
                foreach (Parameter p in RequestObj.Parameters)
                {
                    Debug.WriteLine(String.Format("{0}={1}", p.Name, p.Value));
                }
                 */

                String Base64Result = Execute(RequestObj).Content;
                if (String.IsNullOrEmpty(Base64Result))
                {
                    throw new Exception("No response returned");
                }
                //Debug.WriteLine("Response: " + Base64Result);
                
                // Not sure why sometimes we get double quotes in the response, but trim these
                Base64Result = NormalizeResponseString(Base64Result);
                return (StreamHelper.GetByteBufferFromBase64EncodedString(Base64Result));
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->GetEntityPublicKeyData", e.Message);
                LastError = e.Message;
                return (null);
            }
        }
    }
}
