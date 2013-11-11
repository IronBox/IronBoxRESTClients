using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using RestSharp;
using System.Diagnostics;

namespace LockBox
{
    public class RESTAction_PublicServiceCounters : RESTActionBase
    {
        public RESTAction_PublicServiceCounters(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public long GetPublicServiceCounter(LockBoxStatisticsKey ThisKey, LockBoxAPIVersion APIVersion)
        {
            try
            {
                long id = (long)ThisKey;
                RestRequest request = new RestRequest();
                request.Resource = String.Format("{0}/PublicServiceCounters/{1}", APIVersion.ToString(), id);
                return (Int64.Parse(Execute(request).Content));
            }
            catch (Exception)
            {
                return (0);
            }
        }
    }
}
