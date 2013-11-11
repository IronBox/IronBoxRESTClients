using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RestSharp;
using System.Diagnostics;
using LockBox.Common;
using LockBox.Common.IO;
using System.Net.Http.Formatting;
using LockBox.Common.Security.Cryptography;

namespace LockBox
{
    public partial class LockBoxRESTHelper
    {
        // Form data container data keys
        public static String FormData_ContainerMessageKey = "ContainerMessage";
        

        public static void ApplyContainerMessageToRestRequest(RestRequest ThisRequest, String Message)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_ContainerMessageKey, String.IsNullOrEmpty(Message) ? String.Empty : Message);
            }
        }

        
        public static String GetContainerMessageFromFormData(FormDataCollection FormData)
        {

            return (FormData.Get(FormData_ContainerMessageKey));

        }

        
        
    }

    
}
