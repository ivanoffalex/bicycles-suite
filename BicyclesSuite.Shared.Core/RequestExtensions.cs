using System.Web;

namespace BicyclesSuite.Shared
{
    /// <summary>
    /// Extension methods for HttpRequest
    /// </summary>
    public static class RequestExtensions
    {
        private static readonly string[] _MobileDevices = 
            new [] 
            {
                "iphone", "ipad", "ipod", "ppc",
                "windows ce", "blackberry",
                "opera mini", "mobile", "palm",
                "portable", "opera mobi", "htc", "android" 
            };

        /// <summary>
        /// Detect mobile platform from request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsMobile(this HttpRequest request)
        {
            bool result = request.Browser.IsMobileDevice;
            if (!result && !string.IsNullOrEmpty(request.UserAgent))
            {
                string userAgent = request.UserAgent.ToLower();
                foreach (string dev in _MobileDevices)
                {
                    result = userAgent.Contains(dev);
                    if (result) break;
                }
            }
            return result;
        }

        /// <summary>
        /// Prepare qualified path from request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetFullyQualifiedApplicationPath(this HttpRequest request)
        {
            //Return variable declaration
            string appPath = null;
            //Formatting the fully qualified website url/name
            appPath = string.Format("{0}://{1}{2}{3}",
                request.Url.Scheme,
                request.Url.Host,
                request.Url.Port == 80 ? string.Empty : ":" + request.Url.Port,
                request.ApplicationPath);

            if (!appPath.EndsWith("/"))
                appPath += "/";

            return appPath;
        }

    }
}
