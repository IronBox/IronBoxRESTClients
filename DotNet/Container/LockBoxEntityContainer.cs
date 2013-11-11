using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LockBox.Common.Security.Cryptography;

namespace LockBox
{
    public class EntityContainer
    {
        public long ContainerID { set; get; }
        public String FriendlyID { set; get; }
        public long OwnerEntityID { set; get; }
        public long StorageEndpointID { set; get; }
        public String ContainerStorageName { set; get; }
        public LockBoxContainerProtectionMode ProtectionMode { set; get; }
        public DateTime ExpirationUtc { set; get; }
        public DateTime AvailableUtc { set; get; }
        public SymmetricKeyStrength SymmetricProtection { set; get; }
        public byte[] SymmetricIV { set; get; }
        public AsymmetricKeyStrength AsymmetricProtection { set; get; }
        public String Name { set; get; }
        public String Description { set; get; }
        public String FileNameSalt { set; get; }
        public bool Enabled { set; get; }

        // Default values
        public static String Default_Name = String.Empty;
        public static String Default_Description = String.Empty;
        public static long Default_ContainerID = -1;
        public static String Default_FriendlyID = String.Empty;
        public static long Default_OwnerEntityID = -1;
        public static long Default_StorageEndpointID = -1;
        public static LockBoxContainerProtectionMode Default_ProtectionMode = LockBoxContainerProtectionMode.High;
        public static DateTime Default_ExpirationUtc = DateTime.MaxValue;
        public static DateTime Default_AvailableUtc = DateTime.UtcNow;
        public static SymmetricKeyStrength Default_SymmetricProtection = SymmetricKeyStrength.AES256;
        public static byte[] Default_SymmetricIV = null;
        public static bool Default_Enabled = false;
        public static String Default_ContainerStorageName = String.Empty;

        /*
        // Anonymous access object
        public LockBoxContainerAnonymousAccessData AnonymousAccessData = null;

        // Extended access object
        public LockBoxContainerAPIAccessDataCollection APIAccessData = null;
         */

        public EntityContainer()
        {
            Reset();
        }



        public void Reset()
        {
            ContainerID = Default_ContainerID;
            FriendlyID = Default_FriendlyID;
            OwnerEntityID = Default_OwnerEntityID;
            StorageEndpointID = Default_StorageEndpointID;
            ProtectionMode = Default_ProtectionMode;
            ExpirationUtc = Default_ExpirationUtc;
            AvailableUtc = Default_AvailableUtc;
            SymmetricProtection = Default_SymmetricProtection;
            SymmetricIV = Default_SymmetricIV;
            Name = Default_Name;
            Description = Default_Description;
            ContainerStorageName = Default_ContainerStorageName;
            FileNameSalt = Guid.NewGuid().ToString().Replace("-", String.Empty).Substring(0, 8);
            Enabled = Default_Enabled;

            /*
            // Anonymous access data
            AnonymousAccessData = null;

            // Extended access data
            if (APIAccessData == null)
            {
                APIAccessData = new LockBoxContainerAPIAccessDataCollection();
            }
            APIAccessData.Reset();
             */
        }

        /*
        //---------------------------------------------------------------------
        /// <summary>
        ///     Indicates if anonymous access is allowed on this container
        /// </summary>
        /// <returns>
        ///     Returns true if anonymous access is allowed, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool AnonymousAccessAllowed()
        {
            return (AnonymousAccessData != null);
        }

        public bool EnableAnonymousAccess(LockBoxContainerAnonymousAccessData ThisAnonymousAccessData)
        {
            if (ThisAnonymousAccessData == null)
            {
                return (false);
            }
            else
            {
                // Assign the new anonymous access object
                AnonymousAccessData = ThisAnonymousAccessData;
                return (true);
            }
        }
         */


        public void SetNoExpiration()
        {
            ExpirationUtc = DateTime.MaxValue;
        }



        public bool IsExpired()
        {
            return (DateTime.Compare(DateTime.UtcNow, ExpirationUtc) >= 0);
        }
    }
}
