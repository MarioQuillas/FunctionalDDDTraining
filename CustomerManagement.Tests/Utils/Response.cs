namespace CustomerManagement.Tests.Utils
{
    public class Response
    {
        public Response(string errorMessage, HttpStatusCode statusCode)
        {
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }

        public string ErrorMessage { get; }
        public HttpStatusCode StatusCode { get; }
    }
}