using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RestSharp;
using System.Diagnostics;
using LockBox.Common;

namespace LockBox
{
    public partial class LockBoxRESTHelper
    {

        public static String NormalizeResponseString(String s)
        {
            // JSON returns double quotes around the response, so trim these
            String NewS = s.Trim(new char[] { '"' });


            // Remove any [ or ]
            NewS = NewS.Trim(new char[] { '[', ']' });

            // Any other transformations
            return (NewS);
        }
    }

    
}
