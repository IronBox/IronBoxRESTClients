using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common.Security.Cryptography;

namespace LockBox
{
    public class LockBoxBlobDataStream
    {

        private ILockBoxStorage m_DataStream = null;
        

        public LockBoxBlobDataStream(ILockBoxStorage StorageObject)
        {
            if (StorageObject == null)
            {
                throw new ArgumentNullException();
            }
            m_DataStream = StorageObject;
        }




    }
}
