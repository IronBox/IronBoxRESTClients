using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {
        //---------------------------------------------------------------------
        /// <summary>
        ///     Tests a POST call to the API, for debugging only
        /// </summary>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool PostDebug()
        {
            RESTAction_PostDebug RESTAction = new RESTAction_PostDebug(ServiceBaseUrl);
            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  
            bool Result = RESTAction.PostDebug(APIVersion);
            LastError = RESTAction.LastError;
            return (Result);
        }
    }
}
