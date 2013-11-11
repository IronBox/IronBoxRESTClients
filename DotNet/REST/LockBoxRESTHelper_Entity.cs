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
        public static String FormData_EntityKey = "Entity";
        public static String FormData_EntityTypeKey = "EntityType";
        public static String FormData_EntityPasswordKey = "EntityPassword";


        public static String FormData_Entity2Key = "Entity2";
        public static String FormData_EntityType2Key = "EntityType2";


        public static String FormData_EntitiesCSV = "EntitiesCSV";
        public static String FormData_EntitiesCSVType = "EntitiesCSVType";


        public static void ApplyEntitiesCSVToRestRequest(RestRequest ThisRequest, String EntitiesCSV, LockBoxEntityIDType EntitiesCSVType)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_EntitiesCSV, String.IsNullOrEmpty(EntitiesCSV) ? String.Empty : EntitiesCSV);
                ThisRequest.AddParameter(FormData_EntitiesCSVType, (int)EntitiesCSVType);
            }
        }

        public static void GetEntitiesCSVFromFormData(FormDataCollection FormData, out String EntitiesCSV, out LockBoxEntityIDType EntitiesCSVType)
        {
            EntitiesCSV = FormData.Get(FormData_EntitiesCSV);
            EntitiesCSVType = (LockBoxEntityIDType)Enum.Parse(typeof(LockBoxEntityIDType), FormData.Get(FormData_EntitiesCSVType));
        }


        
        public static void ApplyEntityCredentialsToRestRequest(RestRequest ThisRequest, String Entity, LockBoxEntityIDType IDtype, String Password)
        {
            ApplyEntityToRestRequest(ThisRequest, Entity);
            ApplyEntityTypeToRestRequest(ThisRequest, IDtype);
            ApplyEntityPasswordToRestRequest(ThisRequest, Password);
        }

        public static void ApplyEntityToRestRequest(RestRequest ThisRequest, String Entity)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_EntityKey, String.IsNullOrEmpty(Entity) ? String.Empty : Entity);
            }
        }

        public static void ApplyEntityTypeToRestRequest(RestRequest ThisRequest, LockBoxEntityIDType IDtype)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_EntityTypeKey, (int)IDtype);
            }
        }


        public static void ApplyEntity2ToRestRequest(RestRequest ThisRequest, String Entity)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_Entity2Key, String.IsNullOrEmpty(Entity) ? String.Empty : Entity);
            }
        }

        public static void ApplyEntityType2ToRestRequest(RestRequest ThisRequest, LockBoxEntityIDType IDtype)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_EntityType2Key, (int)IDtype);
            }
        }

        public static void ApplyEntityPasswordToRestRequest(RestRequest ThisRequest, String Password)
        {
            if (ThisRequest != null)
            {
                ThisRequest.AddParameter(FormData_EntityPasswordKey, String.IsNullOrEmpty(Password) ? String.Empty : Password);
            }
        }

        public static String GetEntityFromFormData(FormDataCollection FormData)
        {
            return (FormData.Get(FormData_EntityKey));
        }

        public static LockBoxEntityIDType GetEntityTypeFromFormData(FormDataCollection FormData)
        {
            return ((LockBoxEntityIDType)Enum.Parse(typeof(LockBoxEntityIDType), FormData.Get(FormData_EntityTypeKey)));
        }

        public static String GetEntity2FromFormData(FormDataCollection FormData)
        {
            return (FormData.Get(FormData_Entity2Key));
        }

        public static LockBoxEntityIDType GetEntityType2FromFormData(FormDataCollection FormData)
        {
            return ((LockBoxEntityIDType)Enum.Parse(typeof(LockBoxEntityIDType), FormData.Get(FormData_EntityType2Key)));
        }

        public static String GetEntityPasswordFromFormData(FormDataCollection FormData)
        {
            return (FormData.Get(FormData_EntityPasswordKey));
        }
    }

    
}
