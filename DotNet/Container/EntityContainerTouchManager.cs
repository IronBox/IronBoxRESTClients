using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common;

namespace LockBox
{
    public class EntityContainerTouchManager
    {
        List<String> m_ListA_SeenByEntityContainerTouchCodes = null;
        Dictionary<long,String> m_ListB_CurrentEntityContainerTouchCodes = null;


        public EntityContainerTouchManager()
        {
            m_Reset();
        }

        private void m_Reset()
        {
            m_ListA_SeenByEntityContainerTouchCodes = new List<string>();
            m_ListB_CurrentEntityContainerTouchCodes = new Dictionary<long, string>();
        }

        public bool LoadTouchCodeData(String[] SeenByEntityContainerTouchCodes, Dictionary<long, String> CurrentEntityContainerTouchCodes)
        {
            //  Strategy:
            //      List A = the codes that the entity has recorded that it's seen
            //      List B = the current container touch codes
            //
            //  If a touch code is in both, then seen already
            //  If a touch code is in B, but not A then it's new, and entity hasn't seen it
            //  If a touch code is in A, but not B, then it's stale and should be deleted
            try
            {
                // If any of the inputs are null then toss error
                if (SeenByEntityContainerTouchCodes == null)
                {
                    throw new Exception("Seen by entity object was null");
                }
                if (CurrentEntityContainerTouchCodes == null)
                {
                    throw new Exception("Current entity container touch code info was null");
                }

                m_Reset();

                // Load List A, codes seen by the entity
                m_ListA_SeenByEntityContainerTouchCodes.AddRange(SeenByEntityContainerTouchCodes);
                

                // Load List B, current entity container touch codes
                m_ListB_CurrentEntityContainerTouchCodes = CurrentEntityContainerTouchCodes;
                
                // Remove any stale codes
                m_RemoveStaleCodes();

                return (true);
            }
            catch (Exception e)
            {
                //LockBoxDebugHelper.DoLogToFile(e.ToString());
                return (false);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Removes the codes that are in A, but not B
        /// </summary>
        //---------------------------------------------------------------------
        private void m_RemoveStaleCodes()
        {
            List<String> NewListA = new List<string>();

            foreach (String CurrentCode in m_ListA_SeenByEntityContainerTouchCodes)
            {
                if (m_ListB_CurrentEntityContainerTouchCodes.ContainsValue(CurrentCode))
                {
                    // This code is still current, move it to the new list A
                    NewListA.Add(CurrentCode);
                }
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     
        /// </summary>
        /// <param name="TouchCode"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool IsTouchCodeNew(String TouchCode)
        {
            // If the touch code given is in B, but not A then it's new
            if (String.IsNullOrEmpty(TouchCode))
            {
                return (false);
            }

            bool NotInA =!m_ListA_SeenByEntityContainerTouchCodes.Contains(TouchCode);
            bool InB = m_ListB_CurrentEntityContainerTouchCodes.ContainsValue(TouchCode);

            bool IsNew = NotInA && InB;
            return (IsNew);
        }

        public bool IsEntityContainerNew(long EntityContainerID)
        {
            // Get the entity's touch code and see if it's new, if the entity 
            // container does not exist in our list B, then we assume it's old
            bool Result = false;
            if (m_ListB_CurrentEntityContainerTouchCodes.ContainsKey(EntityContainerID))
            {
                //LockBoxDebugHelper.DoLogToFile("inside");
                String TouchCode = m_ListB_CurrentEntityContainerTouchCodes[EntityContainerID];
                Result = IsTouchCodeNew(TouchCode);
            }
            return (Result);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Returns the new ListA, that is the touch codes seen by the entity
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public String GetCurrentListAasCSV()
        {
            return (StringHelper.CreateCSV(m_ListA_SeenByEntityContainerTouchCodes.ToArray()));
        }


        public void MarkTouchCodeAsSeenByEntity(String TouchCode)
        {
            if (String.IsNullOrEmpty(TouchCode))
            {
                // Nop
                return;
            }

            if (!m_ListA_SeenByEntityContainerTouchCodes.Contains(TouchCode))
            {
                m_ListA_SeenByEntityContainerTouchCodes.Add(TouchCode);
            }
        }

        public bool ContainsContainerID(long ContainerID)
        {
            return (m_ListB_CurrentEntityContainerTouchCodes.ContainsKey(ContainerID));
        }
    }
}
