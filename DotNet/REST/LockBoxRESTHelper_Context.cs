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
        public static String FormData_Context = "Context";

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the context parameter from form data
        /// </summary>
        /// <param name="FormData"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static String GetContextFromFormData(FormDataCollection FormData)
        {
            return (FormData.Get(FormData_Context));
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Adds a context parameter to the given RestRequest
        /// </summary>
        /// <param name="ThisRequest"></param>
        /// <param name="Context"></param>
        //---------------------------------------------------------------------
        public static void ApplyContextToRestRequest(RestRequest ThisRequest, String Context)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_Context, Context);
            }
        }



    }


}
