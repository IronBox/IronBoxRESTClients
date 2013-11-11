using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {


        public bool WriteContainerMessage(String Entity, LockBoxEntityIDType EntityType, String Password, long ContainerID, String Message)
        {
            RESTAction_WriteContainerMessage RESTAction = new RESTAction_WriteContainerMessage(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            bool Result = RESTAction.WriteContainerMessage(APIVersion, Entity, EntityType, Password, ContainerID, Message);
            LastError = RESTAction.LastError;
            return (Result);
            
        }

        


        
    }
}
