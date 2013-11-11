using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class EntityContainerGroupData
    {
        public long GroupID { set; get; }
        public String Name { set; get; }
        public String Description { set; get; }
        public List<long> ContainerMembers { set; get; }


        public EntityContainerGroupData()
        {
            GroupID = -1;
            ContainerMembers = new List<long>();
        }


        public void AddContainerMember(long ContainerID)
        {
            // Only add if it doesn't already contain this ID
            if (!ContainerMembers.Contains(ContainerID))
            {
                ContainerMembers.Add(ContainerID);
            }
        }


        public void RemoveContainerMember(long ContainerID)
        {
            if (ContainerMembers.Contains(ContainerID))
            {
                ContainerMembers.Remove(ContainerID);
            }
        }

        public String GetDBStorableMembershipString()
        {
            String Result = String.Empty;
            foreach (long CurrentID in ContainerMembers)
            {
                Result += String.Format("{0},", CurrentID);
            }

            // Trim any ending , and add a space just in case there
            // are no entries
            Result = Result.Trim(new char[] { ',' }) + " ";

            // Done
            return (Result);
        }

        public bool LoadDBStorableMembershipString(String s)
        {
            // Input validation
            if (String.IsNullOrEmpty(s))
            {
                return (false);
            }

            String StringToUse = s.Trim();
            String[] Tokens = StringToUse.Split(new char[] { ',' });
            if (Tokens != null)
            {
                foreach (String CurrentToken in Tokens)
                {
                    long ContainerID;
                    if (!String.IsNullOrEmpty(CurrentToken) &&
                        Int64.TryParse(CurrentToken,out ContainerID))
                    {
                        AddContainerMember(ContainerID);
                    }
                }
            }
             
            // Done, no error
            return (true);
        }

    }
}
