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
        ///     Indicates if the LockBox API service is resonding
        /// </summary>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool ServiceIsResponding()
        {
            RESTAction_Ping RESTAction = new RESTAction_Ping(ServiceBaseUrl);
            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  
            bool Result = RESTAction.Ping(APIVersion);
            LastError = RESTAction.LastError;
            return (Result);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Returns the value of the specified public service counter
        /// </summary>
        /// <param name="ThisCounter">Counter to return</param>
        /// <returns>
        ///     Specified public service counter
        /// </returns>
        //---------------------------------------------------------------------
        public long GetPublicServiceCounterValue(LockBoxStatisticsKey ThisCounter)
        {
            RESTAction_PublicServiceCounters RESTAction = new RESTAction_PublicServiceCounters(ServiceBaseUrl);
            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  
            long Result = RESTAction.GetPublicServiceCounter(ThisCounter, APIVersion);
            LastError = RESTAction.LastError;
            return (Result);
        }
    }
}
