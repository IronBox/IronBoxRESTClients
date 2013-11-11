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
        public static String FormData_PublicKeyIndexKey = "PublicKeyIndex";
        public static String FormData_PrivateKeyIndexKey = "PrivateKeyIndex";

        public static bool IsValidPublicKeyIndex(LockBoxEntityAsymmetricKey ThisPublicKeyIndex)
        {
            bool Result = false;
            switch (ThisPublicKeyIndex)
            {
                case LockBoxEntityAsymmetricKey.RSA_1024_Public:
                case LockBoxEntityAsymmetricKey.RSA_2048_Public:
                case LockBoxEntityAsymmetricKey.RSA_3072_Public:
                case LockBoxEntityAsymmetricKey.RSA_15360_Public:
                    Result = true;
                    break;

                default:
                    Result = false;
                    break;
            }
            return (Result);
        }


        public static bool IsValidPrivateKeyIndex(LockBoxEntityAsymmetricKey ThisPublicKeyIndex)
        {
            bool Result = false;
            switch (ThisPublicKeyIndex)
            {

                case LockBoxEntityAsymmetricKey.RSA_1024_Private:
                case LockBoxEntityAsymmetricKey.RSA_2048_Private:
                case LockBoxEntityAsymmetricKey.RSA_3072_Private:
                case LockBoxEntityAsymmetricKey.RSA_15360_Private:
                    Result = true;
                    break;

                default:
                    Result = false;
                    break;
            }
            return (Result);
        }

        public static void ApplyPublicKeyIndexToRestRequest(RestRequest ThisRequest, LockBoxEntityAsymmetricKey ThisPublicKeyIndex)
        {
            if ((ThisRequest != null) && IsValidPublicKeyIndex(ThisPublicKeyIndex)) 
            {
                ThisRequest.AddParameter(FormData_PublicKeyIndexKey, (int)ThisPublicKeyIndex);
            }
        }

        public static LockBoxEntityAsymmetricKey GetPublicKeyIndexFromFormData(FormDataCollection FormData)
        {
            return ((LockBoxEntityAsymmetricKey)Enum.Parse(typeof(LockBoxEntityAsymmetricKey), FormData.Get(FormData_PublicKeyIndexKey)));
        }

        public static void ApplyPrivateKeyIndexToRestRequest(RestRequest ThisRequest, LockBoxEntityAsymmetricKey ThisPrivateKeyIndex)
        {
            if ((ThisRequest != null) && IsValidPrivateKeyIndex(ThisPrivateKeyIndex))
            {
                ThisRequest.AddParameter(FormData_PrivateKeyIndexKey, (int)ThisPrivateKeyIndex);
            }
        }

        public static LockBoxEntityAsymmetricKey GetPrivateKeyIndexFromFormData(FormDataCollection FormData)
        {
            return ((LockBoxEntityAsymmetricKey)Enum.Parse(typeof(LockBoxEntityAsymmetricKey), FormData.Get(FormData_PrivateKeyIndexKey)));
        }
        
        
    }

    
}
