using System;
using System.Web;

namespace MenuDelDia.Site.Helpers
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Extension method for the HttpRequestBase class that 
        /// creates the full base Application Url up to and 
        /// including the virtual root (e.g. "http://qa-server/GiveAQuiz")
        /// </summary>
        /// <param name="request">The HttpRequestBase to extend.</param>
        /// <returns>The full base Application Url</returns>    
        public static string ApplicationUrl(this HttpRequestBase request)
        {
            return String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, request.ApplicationPath);
        }

        /// <summary>
        /// Extension method for the HttpRequestBase class that 
        /// creates the full base Url up to but  
        /// not including the virtual root (e.g. "http://qa-server/")
        /// </summary>
        /// <param name="request">The HttpRequestBase to extend.</param>
        /// <returns>The full base Application Url</returns>
        public static string HostUrl(this HttpRequestBase request)
        {
            return String.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
        }

        public static string GetIPAddress(this HttpRequest request)
        {
            var ipList = request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }

            ipList = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }
            var ip = request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(ip) == false)
                return ip;

            return string.Empty;
        }
    }
}
