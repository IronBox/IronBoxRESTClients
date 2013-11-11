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
    public class RESTAction_CreateMailClientSFTContainer : RESTActionBase
    {


        public RESTAction_CreateMailClientSFTContainer(String BaseUrl)
            : base(BaseUrl)
        {
            
        }

        public RESTMailClientContainerConfig CreateMailClientSFTContainer(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType, 
            String Password, String Context, RESTMailClientContainerConfig ContainerConfig)
        {
            try
            {
                // Input validation
                if (String.IsNullOrEmpty(Context))
                {
                    throw new Exception("Input error");
                }
                if (ContainerConfig == null)
                {
                    throw new Exception("Invalid container config object");
                }

                // First create a blob ID
                // Form the REST request, POST {version}/CreateEntityContainerBlob
                RequestObj.Resource = String.Format("{0}/CreateMailClientSFTContainer", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                // Add the entity validation
                LockBoxRESTHelper.ApplyEntityCredentialsToRestRequest(RequestObj, Entity, EntityType, Password);
                LockBoxRESTHelper.ApplyContextToRestRequest(RequestObj, Context);
                
                // Add the container config to the rest request
                LockBoxRESTHelper.ApplyMailClientSFTContainerConfigToRestRequest(RequestObj, ContainerConfig);

                


                // Execute and read the response
                return (Execute<RESTMailClientContainerConfig>(RequestObj));  
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->CreateMailClientSFTContainer", e.Message);
                LastError = e.Message;
                return (null);
            }

            
        }
    }
}
