namespace Teleboard.Common.Exception
{
    public class HttpParameterNotFountException : System.Exception
    {
        public string ParameterName { get; set; }
        public HttpParameterNotFountException(string parameterName)
        {
            ParameterName = parameterName;
        }

        public HttpParameterNotFountException()
        {
        }



    }
}
