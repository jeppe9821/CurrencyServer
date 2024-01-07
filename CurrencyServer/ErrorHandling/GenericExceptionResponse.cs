namespace CurrencyServer.ErrorHandling
{
    public class GenericExceptionResponse
    {
        public int StatusCode { get; set; }
        public string ExceptionMessage { get; set; }
        public string Stacktrace { get; set; }
    }
}
