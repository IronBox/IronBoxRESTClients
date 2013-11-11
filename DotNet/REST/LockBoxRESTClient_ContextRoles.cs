using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {

        public ContextRole[] GetEntityRolesForContext(String Entity, LockBoxEntityIDType EntityType, String Password, String Context)
        {
            RESTAction_GetEntityRolesForContext RESTAction = new RESTAction_GetEntityRolesForContext(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            ContextRole[] Result = RESTAction.GetEntityRolesForContext(APIVersion, Entity, EntityType, Password, Context);
            LastError = RESTAction.LastError;
            return (Result);
        }
    }
}
