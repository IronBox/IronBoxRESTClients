using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {

        public RESTMailClientContainerConfig CreateMailClientSFTContainer(String Entity, LockBoxEntityIDType EntityType, String Password, String Context, RESTMailClientContainerConfig ContainerConfig)
        {

            RESTAction_CreateMailClientSFTContainer RESTAction = new RESTAction_CreateMailClientSFTContainer(ServiceBaseUrl);
            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  
            RESTMailClientContainerConfig Result = RESTAction.CreateMailClientSFTContainer(APIVersion, Entity, EntityType, Password, Context, ContainerConfig);
            LastError = RESTAction.LastError;
            return (Result);
        }

       
    }
}
