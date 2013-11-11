using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RestSharp;
using System.Diagnostics;
using LockBox.Common;
using System.Net.Http.Formatting;

namespace LockBox
{
    public partial class LockBoxRESTHelper
    {
        // Entity validation keys
        public static String FormData_ListCSVKey = "ListCSV";
        
        
        

        public static void ApplyListCSVToRestRequest(RestRequest ThisRequest, String[] ListCSV)
        {
            String CSV = StringHelper.CreateCSV(ListCSV);
            if ((ThisRequest != null) && !String.IsNullOrEmpty(CSV))
            {
                ThisRequest.AddParameter(FormData_ListCSVKey, CSV);
            }
        }


        public static String[] GetListCSVFromFormData(FormDataCollection FormData)
        {
            String RawString = FormData.Get(FormData_ListCSVKey);
            return (RawString.Split(new char[] { ',' }));
        }

        
    }

    
}
