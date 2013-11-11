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
        public static String FormData_ContextSetting = "ContextSetting";

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the context setting parameter from form data
        /// </summary>
        /// <param name="FormData"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static ContextSetting GetContextSettingFromFormData(FormDataCollection FormData)
        {
            String SettingsRaw = FormData.Get(FormData_ContextSetting);
            ContextSetting ParsedContextSetting;
            if (!Enum.TryParse<ContextSetting>(SettingsRaw, out ParsedContextSetting))
            {
                throw new Exception("Unable to parse setting from form data");
            }
            return (ParsedContextSetting);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Adds a context setting to the form data
        /// </summary>
        /// <param name="ThisRequest"></param>
        /// <param name="Context"></param>
        //---------------------------------------------------------------------
        public static void ApplyContextSettingToRestRequest(RestRequest ThisRequest, ContextSetting Setting)
        {
            if (ThisRequest != null)
            {               
                // Add it to the request as a parameter
                ThisRequest.AddParameter(FormData_ContextSetting, Setting.ToString());
            }
        }


        public static bool IsAuthorizedContextSetting(ContextSetting ThisSetting)
        {
            bool Result = false;

            switch (ThisSetting)
            {
                case ContextSetting.CompanyLogoUrl:
                    Result = true;
                    break;

                // List additional authorized settings here, all others are unauthorized

                default:
                    Result = false;
                    break;
            }
            return (Result);
        }
    }


}
