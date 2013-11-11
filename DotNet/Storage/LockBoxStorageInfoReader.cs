using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

namespace LockBox
{
    /// <summary>
    ///     Class that abstracts information retrieved by the EntityManager about 
    ///     storage endpoint information
    /// </summary>
    public class LockBoxStorageInfoReader
    {

        private Dictionary<long, PhysicalStorageLocation> m_LocationInfo = null;
        private Dictionary<long, CloudStorageVendor> m_VendorInfo = null;

        //---------------------------------------------------------------------
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="InfoCSV">CSV info string to load</param>
        //---------------------------------------------------------------------
        public LockBoxStorageInfoReader(String InfoCSV)
        {
            m_LocationInfo = new Dictionary<long, PhysicalStorageLocation>();
            m_VendorInfo = new Dictionary<long, CloudStorageVendor>();
            if (!m_LoadStorageInfoCSV(InfoCSV))
            {
                throw new Exception("Invalid information CSV value, can't parse");
            }
            
        }


        public long[] GetAllStorageEndpointIds()
        {
            return (m_VendorInfo.Keys.ToArray());
        }

        public PhysicalStorageLocation GetLocation(long StorageEndpointID)
        {
            return (m_LocationInfo[StorageEndpointID]);
        }

        public CloudStorageVendor GetVendor(long StorageEndpointID)
        {
            return (m_VendorInfo[StorageEndpointID]);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Parses the storage information CSV
        /// </summary>
        /// <param name="InfoCSV"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        private bool m_LoadStorageInfoCSV(String InfoCSV)
        {
            try
            {
                // Clear previous stuff
                m_LocationInfo.Clear();
                m_VendorInfo.Clear();

                // Info strings are formatted: id:location:vendor,id:location:vendor
                // Split the tokens by comma first to get each storage endpoint
                String[] StorageEndpointInfo = InfoCSV.Split(new char[] { ',' });
                foreach (String StorageToken in StorageEndpointInfo)
                {
                    String[] StorageProperties = StorageToken.Split(new char[] { ':' });
                    long ID = Int64.Parse(StorageProperties[0]);
                    PhysicalStorageLocation Location = (PhysicalStorageLocation)Enum.Parse(typeof(PhysicalStorageLocation), StorageProperties[1]);
                    CloudStorageVendor Vendor = (CloudStorageVendor)Enum.Parse(typeof(CloudStorageVendor), StorageProperties[2]);
                    
                    // Add the info
                    m_VendorInfo[ID] = Vendor;
                    m_LocationInfo[ID] = Location;

                    // Next storage endpoint
                }

                // Done
                return (true);
            }
            catch (Exception)
            {

                return (false);
            }
        }
    }
}
