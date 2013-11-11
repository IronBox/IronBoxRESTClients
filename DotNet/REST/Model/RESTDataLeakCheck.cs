using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common.Analysis;

namespace LockBox
{
    public class RESTDataLeakCheck
    {
        

        public String Name { set; get; }
        public AnalysisCheckType CheckType { set; get; }
        public String CheckData { set; get; }


        
    }
}
