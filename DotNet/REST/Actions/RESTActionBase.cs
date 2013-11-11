using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RestSharp;
using System.Diagnostics;
using System.IO;

namespace LockBox
{
    public class RESTActionBase
    {
        private String m_BaseUrl = null;
        public String BaseUrl
        {
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Base URL may not be null or empty");
                }
                m_BaseUrl = value;
            }
            get
            {
                return (m_BaseUrl);
            }
        }

        protected RestRequest RequestObj = null;

        public String LogFilePath { set; get; }
        
        public RESTActionBase(String BaseUrl)
        {
            this.BaseUrl = BaseUrl;
            RequestObj = new RestRequest();

        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Sets or get the request format, XML or JSON
        /// </summary>
        //---------------------------------------------------------------------
        public DataFormat RequestFormat
        {
            set
            {
                // Clear the accept header
                RequestObj.RequestFormat = value;

                // Force to use xml or json
                //RequestObj.AddHeader("Accept", "text/xml");
                //RequestObj.AddHeader("Accept", "application/json");
            }

            get
            {
                return (RequestObj.RequestFormat);
            }
        }
        


        private RestClient m_GetInitializedRestClient()
        {
            var client = new RestClient();
            client.BaseUrl = BaseUrl;
            return (client);
        }



        public RestResponse Execute(RestRequest request)
        {
            var client = m_GetInitializedRestClient();

            m_LogRestRequest(request);

            /*
            return ((RestResponse)client.Execute(request));
             */
            RestResponse response = (RestResponse)client.Execute(request);
            //Debug.WriteLine(response.Content);
            m_LogRestResponse(response);
            return (response);
        }



        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = m_GetInitializedRestClient();

            // Log the request
            m_LogRestRequest(request);


            RestResponse<T> Result = (RestResponse<T>)client.Execute<T>(request);
            Debug.WriteLine(Result.Content);
            m_LogRestResponse<T>(Result);
            return (Result.Data); 
            
            /*
            T Result = client.Execute<T>(request).Data;
            Debug.WriteLine("Result was null: " + (Result == null));
            return (Result);
             */
            
        }


        public String NormalizeResponseString(String s)
        {
            /*
            // JSON returns double quotes around the response, so trim these
            String NewS = s.Trim(new char[] { '"' });


            // Remove any [ or ]
            NewS = NewS.Trim(new char[] { '[', ']' });

            // Any other transformations
            return (NewS);
             */
            return (LockBoxRESTHelper.NormalizeResponseString(s));
        }


        public String LastError { set; get; }



        private void m_LogRestRequest(RestRequest Request)
        {
            try
            {
                if (!String.IsNullOrEmpty(LogFilePath))
                {
                    var body = Request.Parameters.Where(p => p.Type == ParameterType.RequestBody).FirstOrDefault();
                    m_DoGenericLog(body.Value.ToString(), "REQUEST");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                // Nop
            }
        }


        private void m_LogRestResponse(RestResponse Response)
        {
            try
            {
                if (!String.IsNullOrEmpty(LogFilePath))
                {
                    String body = Response.Content;
                    m_DoGenericLog(body, "RESPONSE");
                }
            }
            catch (Exception)
            {
                // Nop
            }
        }

        private void m_LogRestResponse<T>(RestResponse<T> Response)
        {
            try
            {
                if (!String.IsNullOrEmpty(LogFilePath))
                {
                    String body = Response.Content;
                    m_DoGenericLog(body, "RESPONSE");
                }
            }
            catch (Exception)
            {
                // Nop
            }
        }


        private void m_DoGenericLog(String Message, String Header)
        {
            if (!String.IsNullOrEmpty(Message))
            {
                m_AppendToFile(LogFilePath, String.Format("---START-{0}---", Header));
                m_AppendToFile(LogFilePath, Message);
                m_AppendToFile(LogFilePath, String.Format("---END-{0}---\n\n", Header));
            }
        }


        private void m_AppendToFile(String FilePath, String Message)
        {
            // We need to be careful that if the path doesn't exist, then it will toss an error
            StreamWriter sw = null;
            try
            {
                sw = File.AppendText(FilePath);
                sw.WriteLine(Message);
                sw.Flush();
            }
            catch (Exception)
            {
                // Nop
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }

        }
        
    }

    
    

}
