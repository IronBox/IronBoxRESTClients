using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum LockBoxEntityAsymmetricKey : int
    {
        RSA_1024_Private = 0,
        RSA_1024_Public,

        RSA_2048_Private,
        RSA_2048_Public,

        RSA_3072_Private,
        RSA_3072_Public,

        RSA_15360_Private,
        RSA_15360_Public,
    }
}
