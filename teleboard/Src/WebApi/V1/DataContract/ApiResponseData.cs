namespace Teleboard.UI.WebApi.V1.DataContract
{
    public class ApiResponseData
    {
        public int code { get; set; }
        public string description { get; set; }
        public object data { get; set; }
    }
}