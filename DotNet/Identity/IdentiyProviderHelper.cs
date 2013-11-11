using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class IdentiyProviderHelper
    {
        public static bool TryParseIdentityProvider(String s, out IdentityProviders Result)
        {
            try
            {
                if (String.IsNullOrEmpty(s))
                {
                    throw new Exception("Invalid input");
                }

                String StoUse = s.ToLower().Trim();
                switch (StoUse)
                {  
                    case "google":
                        Result = IdentityProviders.Google;
                        break;

                    // Add other supported providers here

                    default:
                        throw new Exception("Unknown identity provider");
                        
                }

                return (true);
            }
            catch (Exception)
            {
                Result = IdentityProviders.None;
                return (false);
            }
        }
    }
}
