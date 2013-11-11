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
using System.Security.Cryptography;

namespace LockBox
{
    public partial class LockBoxRESTHelper
    {
        
        public static String FormData_BlobNameKey = "BlobName";
        public static String FormData_BlobIDKey = "BlobIDName";
        public static String FormData_BlobSizeBytes = "BlobSizeBytes";
        public static String FormData_BlobCheckInToken = "BlobCheckInToken";
        public static String FormData_BlobState = "BlobState";


        public static void ApplyBlobStateToRestRequest(RestRequest ThisRequest, LockBoxBlobStatusKey State)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_BlobState, (uint)State);
            }
        }

        public static RESTBlobInfo ConvertInternalBlobEntryToRESTBlobInfo(EntityContainerBlobEntry BlobEntry)
        {
            try
            {
                // Basic input validation
                if (BlobEntry == null)
                {
                    throw new Exception("Blob entry was null");
                }


                // Read to convert
                RESTBlobInfo Result = new RESTBlobInfo();
                Result.BlobID = BlobEntry.BlobID;
                Result.BlobName = BlobEntry.BlobName;

                // Done, add anything else that should be public here ...

                return (Result);
            }
            catch (Exception)
            {
                return (null);
            }
        }

        public static LockBoxBlobStatusKey GetBlobStatusKeyFromFormData(FormDataCollection FormData)
        {
            return ((LockBoxBlobStatusKey)UInt32.Parse(FormData.Get(FormData_BlobState)));
        }


        public static void ApplyBlobNameToRestRequest(RestRequest ThisRequest, String BlobName)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_BlobNameKey, BlobName);
            }
        }


        public static void ApplyBlobIDToRestRequest(RestRequest ThisRequest, String BlobID)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_BlobIDKey, BlobID);
            }
        }

        public static void ApplyBlobSizeBytesToRestRequest(RestRequest ThisRequest, long SizeBytes)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_BlobSizeBytes, SizeBytes.ToString());
            }
        }

        public static void ApplyBlobCheckInTokenToRestRequest(RestRequest ThisRequest, String CheckInToken)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_BlobCheckInToken, CheckInToken);
            }
        }

        public static void ApplyBlobCheckInDataToRestRequest(RestRequest ThisRequest, EntityBlobCheckInData CheckInData)
        {
            ApplyBlobSizeBytesToRestRequest(ThisRequest, CheckInData.SizeBytes);
            ApplyBlobCheckInTokenToRestRequest(ThisRequest, CheckInData.CheckInToken);
        }

        public static EntityBlobCheckInData GetBlobCheckInDataFromFormData(FormDataCollection FormData)
        {
            EntityBlobCheckInData Result = new EntityBlobCheckInData();
            Result.SizeBytes = GetBlobSizeBytesFromFormData(FormData);
            Result.CheckInToken = GetBlobCheckInTokenFromFormData(FormData);
            return (Result);
        }

        public static String GetBlobCheckInTokenFromFormData(FormDataCollection FormData)
        {
            return (FormData.Get(FormData_BlobCheckInToken));
        }

        public static long GetBlobSizeBytesFromFormData(FormDataCollection FormData)
        {
            return (Int64.Parse(FormData.Get(FormData_BlobSizeBytes)));
        }


        public static String GetBlobNameFromFormData(FormDataCollection FormData)
        {
            return (NormalizeResponseString(FormData.Get(FormData_BlobNameKey)));
        }

        public static String GetBlobIDFromFormData(FormDataCollection FormData)
        {
            return (NormalizeResponseString(FormData.Get(FormData_BlobIDKey)));
        }
        
    }

    
}
