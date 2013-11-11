using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace LockBox
{
    [DataContract(Name = "LockBoxErrorCode")]
    public enum LockBoxErrorCode : uint
    {
        [EnumMember]
        NotSet = 0,

        [EnumMember]
        NoError,

        [EnumMember]
        CryptoError,

        [EnumMember]
        ValidationError,

        [EnumMember]
        AuthorizationError,

        [EnumMember]
        NetworkError,

        [EnumMember]
        InvalidInput,

        [EnumMember]
        InvalidResponseData,

        [EnumMember]
        InvalidCommand,

        [EnumMember]
        EntityDoesNotExist,

        [EnumMember]
        CommandFailed,

        [EnumMember]
        StorageEndpointDoesNotExist,

        [EnumMember]
        ContainerFriendlyIDAlreadyExists,

        [EnumMember]
        CommandParameterMissing,

        [EnumMember]
        ContainerDoesNotExist,

        [EnumMember]
        BlobDoesNotExist,

        [EnumMember]
        BlobAlreadyExists,

        [EnumMember]
        EntityNotAMember,

        [EnumMember]
        EntityAlreadyAMember,
    }
}
