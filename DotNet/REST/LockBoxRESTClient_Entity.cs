using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LockBox
{
    public partial class LockBoxRESTClient
    {

        public String[] CheckEntitiesContextMembershipInRole(String Context, String EntitiesCSV, LockBoxEntityIDType EntitiesCSVType, ContextRole ThisRole)
        {
            RESTAction_CheckEntitiesContextMembershipInRole RESTAction = new RESTAction_CheckEntitiesContextMembershipInRole(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            String[] Result = RESTAction.CheckEntitiesContextMembership(APIVersion, Context, EntitiesCSV, EntitiesCSVType, ThisRole);
            LastError = RESTAction.LastError;
            return (Result);
        }

        
        public long ResolveEntityToID(String Entity, LockBoxEntityIDType EntityType)
        {
            RESTAction_ResolveEntityToID RESTAction = new RESTAction_ResolveEntityToID(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            long Result = RESTAction.ResolveEntityToID(APIVersion, Entity, EntityType);
            LastError = RESTAction.LastError;
            return (Result);
        }       
    }
}
