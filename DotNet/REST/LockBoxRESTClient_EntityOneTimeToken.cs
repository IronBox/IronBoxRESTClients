using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {


        public RESTEntityOneTimeTokenData GetEntityOneTimeToken(String Entity, LockBoxEntityIDType EntityType, String Password)
        {
            RESTAction_CreateEntityOneTimeToken RESTAction = new RESTAction_CreateEntityOneTimeToken(ServiceBaseUrl);
            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            RESTEntityOneTimeTokenData RESTResult = RESTAction.CreateEntityOneTimeToken(APIVersion, Entity, EntityType, Password);            

            // Done, return the result
            return (RESTResult);
        }
    }
}
