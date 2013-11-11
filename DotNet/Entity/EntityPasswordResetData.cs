using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class EntityPasswordResetData
    {
        public static uint DefaultTTLDays = 1;

        public String ResetToken { set; get; }
        public DateTime UtcTimeStamp { set; get; }


        public EntityPasswordResetData()
        {
            ResetToken = Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Indicates if the reset token is expired or not
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool IsExpired()
        {
            // Default TTL is 1 day
            return (IsExpired(DefaultTTLDays));
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     
        /// </summary>
        /// <param name="TTLDays"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool IsExpired(uint TTLDays)
        {
            return (DateTime.Compare(DateTime.UtcNow, UtcTimeStamp.AddDays(TTLDays)) > 0);
        }

        public bool ValidateToken(String Token)
        {
            return (ValidateToken(Token, DefaultTTLDays));
        }


        public bool ValidateToken(String Token, uint TTL)
        {
            return (!String.IsNullOrEmpty(Token) &&
                (Token == ResetToken) &&
                !IsExpired(TTL));
        }

    }
}
