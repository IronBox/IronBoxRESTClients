using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum ContainerMessageType : int
    {
        EncryptedConversation = 0,
        CleartextLog,
    }
}
