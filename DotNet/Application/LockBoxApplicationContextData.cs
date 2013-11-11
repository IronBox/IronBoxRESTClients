using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class LockBoxApplicationContextData
    {
        //---------------------------------------------------------------------
        /// <summary>
        ///     Sets or gets the context ID
        /// </summary>
        //---------------------------------------------------------------------
        public long ContextID { set; get; }


        private String m_Value = String.Empty;

        //---------------------------------------------------------------------
        /// <summary>
        ///     Sets or gets the context data value
        /// </summary>
        //---------------------------------------------------------------------
        public String Value
        {
            set
            {
                // Values must be lowercase trimmed per specifications
                m_Value = String.IsNullOrEmpty(value) ? String.Empty : NormalizeContext(value);
            }

            get
            {
                return (m_Value);
            }
        }


        public Dictionary<LockBoxApplicationContextSetting, String> ContextSettings
        {
            set;
            get;
        }

        public static String NormalizeContext(String s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return (s);
            }
            else
            {
                return (s.ToLower().Trim());
            }
        }
    }
}
