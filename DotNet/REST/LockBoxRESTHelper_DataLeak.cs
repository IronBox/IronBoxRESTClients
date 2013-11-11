using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common.Analysis;
using LockBox;

namespace LockBox
{
    public partial class LockBoxRESTHelper
    {

        //---------------------------------------------------------------------
        /// <summary>
        ///     Converts an array of RESTDataLeakChecks to an array of 
        ///     AnalysisCheck objects
        /// </summary>
        /// <param name="RESTDataLeakChecks"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static AnalysisCheck[] ConvertRESTDataLeakChecksToAnalysisCheckObjects(RESTDataLeakCheck[] RESTDataLeakChecks)
        {
            try
            {
                List<AnalysisCheck> Results = new List<AnalysisCheck>();
                foreach (RESTDataLeakCheck CurrentCheck in RESTDataLeakChecks)
                {
                    Results.Add(m_ConvertRESTDataleakCheckToAnalysisCheck(CurrentCheck));
                }
                return (Results.ToArray());
            }
            catch (Exception)
            {
                return (null);
            }

        }


        private static AnalysisCheck m_ConvertRESTDataleakCheckToAnalysisCheck(RESTDataLeakCheck ThisCheck)
        {
            AnalysisCheck Result = new AnalysisCheck();
            Result.Name = ThisCheck.Name;
            Result.CheckType = ThisCheck.CheckType;
            Result.CheckData = ThisCheck.CheckData;

            // Do any additional parsing based on the type ...

            return (Result);
        }



        public static RESTDataLeakCheck[] ConvertAnalysisCheckToRESTDataLeakCheckObjects(AnalysisCheck[] TheseChecks)
        {
            try
            {
                List<RESTDataLeakCheck> Results = new List<RESTDataLeakCheck>();
                foreach (AnalysisCheck CurrentCheck in TheseChecks)
                {
                    Results.Add(m_ConvertAnalysisCheckToRESTDataleakCheck(CurrentCheck));
                }
                return (Results.ToArray());
            }
            catch (Exception)
            {
                return (null);
            }

        }


        private static RESTDataLeakCheck m_ConvertAnalysisCheckToRESTDataleakCheck(AnalysisCheck ThisCheck)
        {
            RESTDataLeakCheck Result = new RESTDataLeakCheck();
            Result.Name = ThisCheck.Name;
            Result.CheckType = ThisCheck.CheckType;
            Result.CheckData = ThisCheck.CheckData.ToString();

            // Do any additional parsing based on the type ...

            return (Result);
        }
        
    }

    
}
