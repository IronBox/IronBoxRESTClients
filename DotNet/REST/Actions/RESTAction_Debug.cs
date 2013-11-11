using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using RestSharp;
using System.Diagnostics;

namespace LockBox
{
    public class RESTAction_PostDebug : RESTActionBase
    {
        public RESTAction_PostDebug(String BaseUrl)
            : base(BaseUrl)
        {

        }

        public bool PostDebug(LockBoxAPIVersion APIVersion)
        {
            try
            {
                RestRequest request = new RestRequest();
                request.Resource = String.Format("{0}/Debug", APIVersion.ToString());
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/x-ww-form-urlencoded");
                String Value = Guid.NewGuid().ToString().ToLower();
                request.AddParameter("Value", Value);
                String ReturnedString = Execute(request).Content;
                Debug.WriteLine("Returned string was = " + ReturnedString);
                return (NormalizeResponseString(ReturnedString) == Value.ToUpper());
            }
            catch (Exception)
            {
                return (false);
            }
        }
    }
}
