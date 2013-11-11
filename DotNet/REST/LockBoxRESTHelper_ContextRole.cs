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
        public static String FormData_ContextRole = "ContextRole";

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the context role parameter from form data
        /// </summary>
        /// <param name="FormData"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static ContextRole GetContextRoleFromFormData(FormDataCollection FormData)
        {
            return ((ContextRole)Enum.Parse(typeof(ContextRole), FormData.Get(FormData_ContextRole)));
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Adds a context role parameter to the given RestRequest
        /// </summary>
        /// <param name="ThisRequest"></param>
        /// <param name="Context"></param>
        //---------------------------------------------------------------------
        public static void ApplyContextRoleToRestRequest(RestRequest ThisRequest, ContextRole ContextRole)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_ContextRole, ContextRole.ToString());
            }
        }



    }


}
