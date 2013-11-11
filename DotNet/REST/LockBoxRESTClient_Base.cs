using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {
        public String LastError { set; get; }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Sets or gets the version to use
        /// </summary>
        //---------------------------------------------------------------------
        public LockBoxAPIVersion APIVersion { set; get; }

        public String ServiceBaseUrl
        {
            set;
            get;
        }

        public LockBoxRestClientRequestFormat RequestFormat { set; get; }


        public String LogFilePath { set; get; }

        public LockBoxRESTClient(String ServiceBaseUrl, LockBoxAPIVersion ServiceVersion)
        {
            m_DoInit(ServiceBaseUrl, ServiceVersion);
        }

        public LockBoxRESTClient(String ServiceBaseUrl)
        {
            // Default to the latest version of the API
            m_DoInit(ServiceBaseUrl, LockBoxAPIVersion.Latest);
        }

        private void m_DoInit(String ServiceBaseUrl, LockBoxAPIVersion ServiceVersion)
        {
            this.ServiceBaseUrl = ServiceBaseUrl;
            APIVersion = ServiceVersion;
            LastError = String.Empty;

            // Caching objects
            m_InitCaches();
            EnableCaching = false;

            // Default this to XML
            RequestFormat = LockBoxRestClientRequestFormat.Xml;
        }


        private void m_ConfigureRESTActionBase(RESTActionBase ThisBaseAction)
        {
            // Set the request format
            RestSharp.DataFormat TargetDataFormat = RestSharp.DataFormat.Xml;
            switch (RequestFormat)
            {
                case LockBoxRestClientRequestFormat.Json:
                    TargetDataFormat = RestSharp.DataFormat.Json;
                    break;

                case LockBoxRestClientRequestFormat.Xml:
                    TargetDataFormat = RestSharp.DataFormat.Xml;
                    break;

                default:
                    throw new NotImplementedException("Local REST request format not implemented");
                    
            }
            ThisBaseAction.RequestFormat = TargetDataFormat;

            // Set the log file path
            ThisBaseAction.LogFilePath = LogFilePath;
        }
    }
}
