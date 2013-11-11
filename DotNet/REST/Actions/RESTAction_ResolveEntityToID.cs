using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using RestSharp;
using System.Diagnostics;
using LockBox.Common;
using LockBox.Common.IO;


namespace LockBox
{
    public class RESTAction_ResolveEntityToID : RESTActionBase
    {
        public RESTAction_ResolveEntityToID(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public long ResolveEntityToID(LockBoxAPIVersion APIVersion, String Entity, LockBoxEntityIDType EntityType)
        {
            try
            {
                // Form the REST request
                RequestObj.Resource = String.Format("{0}/ResolveEntityToID", APIVersion.ToString());
                RequestObj.Method = Method.POST;

                LockBoxRESTHelper.ApplyEntityTypeToRestRequest(RequestObj, EntityType);
                LockBoxRESTHelper.ApplyEntityToRestRequest(RequestObj, Entity);

                // Execute the command
                return (Int64.Parse(Execute(RequestObj).Content));
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("REST_Action->ResolveEntityToID", e.Message);
                LastError = e.Message;
                return (-1);
            }
        }
    }
}
