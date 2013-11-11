using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common;
using LockBox.Common.Collections.Data;

namespace LockBox
{
    public class LockBoxContainerRightsCollection : List<LockBoxContainerRights>
    {

        public static char Delimiter = '&';


        public LockBoxContainerRightsCollection()
            : base()
        {

        }

        public void AddRight(LockBoxContainerRights R)
        {
            if (!ContainsRight(R))
            {
                this.Add(R);
            }
        }

        public void RemoveRight(LockBoxContainerRights R)
        {
            if (ContainsRight(R))
            {
                this.Remove(R);
            }
        }

        public bool ContainsRight(LockBoxContainerRights R)
        {
            return (this.Contains(R));
        }


        public String GetDBStorageString()
        {
            String Result = String.Empty;
            foreach (LockBoxContainerRights R in this)
            {
                Result += String.Format("{0}{1}", R.ToString(), Delimiter);
            }
            Result = Result.Trim(new char[] { Delimiter });   // Remove any trailing &
            return (Result);
        }

        public bool LoadDBStorableString(String S)
        {
            //LockBoxDebugHelper.Debug_Log("trying to load: ", S, false);
            this.Clear();
            try
            {
                String[] Tokens = S.Split(new char[]{ Delimiter });
                foreach (String s in Tokens)
                {
                    this.Add((LockBoxContainerRights)Enum.Parse(typeof(LockBoxContainerRights), s));
                }
                return (true);
            }
            catch (Exception)
            {
                return (false);
            }
        }

        public void AddAllPossibleRights()
        {
            foreach (LockBoxContainerRights R in Enum.GetValues(typeof(LockBoxContainerRights)))
            {
                this.Add(R);
            }
        }


        public String ToCSV()
        {
            return (GetDBStorageString().Replace(Delimiter, ','));
        }
    }
}
