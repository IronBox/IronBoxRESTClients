using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using RestSharp;
using System.Diagnostics;

namespace LockBox
{
    public class RESTAction_Ping : RESTActionBase
    {
        public RESTAction_Ping(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public bool Ping(LockBoxAPIVersion APIVersion)
        {
            try
            {
                RequestObj.Resource = String.Format("{0}/Ping", APIVersion.ToString());
                return (Boolean.Parse(Execute(RequestObj).Content));
            }
            catch (Exception e)
            {
                return (false);
            }
        }
    }
}
