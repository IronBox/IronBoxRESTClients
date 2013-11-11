using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {
        public bool SetEntityContainerMembership(String Entity, LockBoxEntityIDType EntityType, String Password, long ContainerID, String Entity2, LockBoxEntityIDType Entity2Type,
            LockBoxContainerRightsCollection ApplyTheseRights)
        {
            RESTAction_SetEntityContainerMembership RESTAction = new RESTAction_SetEntityContainerMembership(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            bool Result = RESTAction.SetEntityContainerMembership(APIVersion, Entity, EntityType, Password, ContainerID, Entity2, Entity2Type, ApplyTheseRights);
            LastError = RESTAction.LastError;
            return (Result);
        }


        public bool RemoveEntityContainerMembership(String Entity, LockBoxEntityIDType EntityType, String Password, long ContainerID, String Entity2, LockBoxEntityIDType Entity2Type)
        {
            RESTAction_RemoveEntityContainerMembership RESTAction = new RESTAction_RemoveEntityContainerMembership(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            bool Result = RESTAction.RemoveEntityContainerMembership(APIVersion, Entity, EntityType, Password, ContainerID, Entity2, Entity2Type);
            LastError = RESTAction.LastError;
            return (Result);
        }
    }
}
