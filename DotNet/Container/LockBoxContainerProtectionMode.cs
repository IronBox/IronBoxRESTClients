using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace LockBox
{
    [DataContract(Name = "LockBoxContainerProtectionMode")]
    public enum LockBoxContainerProtectionMode : int
    {
        [EnumMember]
        Standard = 0,
        [EnumMember]
        Moderate,
        [EnumMember]
        High,

        // Container for protecting cloud third party data providers (dropbox, skydrive, google drive)
        [EnumMember]
        ThirdPartyCloudDataProvider,

        // Container for protecting only passwords, future
        [EnumMember]
        PasswordProtector,

        [EnumMember]
        RetailAES256Container,          // These are the containers that are used by the retail IronCloud site
    }
}
