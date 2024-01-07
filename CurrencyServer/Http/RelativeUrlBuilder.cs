namespace CurrencyServer.Http
{
    public class RelativeUrlBuilder
    {
        private string _relativePath = string.Empty;
        private IDictionary<string, string> _queryParams = new Dictionary<string, string>();

        /// <summary>
        /// Adds a path to the URL
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public RelativeUrlBuilder AddPath(string path)
        {
            _relativePath = Path.Combine(_relativePath, path);
            return this;
        }

        /// <summary>
        /// Adds a query to the URL with a name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public RelativeUrlBuilder AddQuery(string name, string value)
        {
            _queryParams[name] = value;
            return this;
        }

        /// <summary>
        /// Builds the relative URL
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            var queries = string.Join("&", _queryParams.Select(x => $"{x.Key}={x.Value}"));

            var fullPath = queries?.Length > 0 ? 
                $"{_relativePath}?{queries}" : 
                _relativePath;

            return fullPath;
        }
    }
}
