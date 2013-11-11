using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {

        public RESTDataLeakCheck[] GetDataLeakChecks(String Entity, LockBoxEntityIDType EntityType, String Password, String Context)
        {
            RESTAction_GetDataLeakChecks RESTAction = new RESTAction_GetDataLeakChecks(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            RESTDataLeakCheck[] Result = RESTAction.GetDataLeakChecks(APIVersion, Entity, EntityType, Password, Context);
            LastError = RESTAction.LastError;
            return (Result);
        }

        
    }
}
