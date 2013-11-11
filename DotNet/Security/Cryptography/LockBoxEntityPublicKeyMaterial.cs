using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class LockBoxEntityPublicKeyMaterial
    {
        public byte[] RSA_1024_Public;
        public byte[] RSA_2048_Public;
        public byte[] RSA_3072_Public;
        public byte[] RSA_15360_Public;

        // Extended key material
        public byte[] PasswordSalt;
        public byte[] ProtectionIV;
        public int KeyDerivationIterations;

        public LockBoxEntityPublicKeyMaterial()
        {
            KeyDerivationIterations = -1;
        }
    }
}
