using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace LockBox
{
    [DataContract(Name = "LockBoxContainerPropertyKey")]
    public enum LockBoxContainerPropertyKey : int
    {
        
        //[EnumMember]
        AutoRenameFiles = 0,
 
        AllowLogging,
        AllowEncryptedConversations,


        UploadNotificationEmailList,
        DownloadNotificationEmailList,

        AllowAPI,
        AllowEasyAccessSecureMessages,


        // Flag that indicates if expiration notification is sent
        ExpirationNotificationSentBoolFlag,

        // Indicates that a mandatory expiration has been set, so cannot change this, 
        // this has a true/false setting, but it's mere existence tells us it must expire
        MandatoryContainerExpirationFlag,


        // IpAddress, white list
        AccessIpAddressWhitelist,


        // Pay as you go container flag, boolean
        IsPayAsYouGoContainer,


        // Additional container properties, maintain order
    }
}
