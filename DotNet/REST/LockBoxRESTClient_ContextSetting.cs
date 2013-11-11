using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {

        public String GetContextSetting(String Entity, LockBoxEntityIDType EntityType, String Password, String Context, ContextSetting ThisSetting)
        {
            RESTAction_GetContextSetting RESTAction = new RESTAction_GetContextSetting(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            String Result = RESTAction.GetContextSetting(APIVersion, Entity, EntityType, Password, Context, ThisSetting);
            LastError = RESTAction.LastError;
            return (Result);
        }
    }
}
