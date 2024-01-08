namespace CurrencyServer.Http
{
    /// <summary>
    /// General helper class for URL handling
    /// </summary>
    public static class UrlHelper
    {
        private const string EndingToken = "/";


        /// <summary>
        /// Fixes the base address for a base url. The HttpClient BaseAddress requires the last character to be '/'. To circumvent the configuration lacking the '/' token at the end, this method is used
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string FixBaseAddress(string url)
        {
            if(string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url), "Url cannot be empty or null");
            }

            if(url.EndsWith(EndingToken))
            {
                return url;
            }

            return string.Concat(url, EndingToken);
        }
    }
}
