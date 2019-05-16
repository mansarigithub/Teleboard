using Teleboard.Common.Enum;

namespace Teleboard.Common.Exception
{
    public class BusinessException : System.Exception
    {
        public BusinessExceptionType Type { get; set; }

        public BusinessException() : base()
        {
        }

        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(BusinessExceptionType type) : base(type.ToString())
        {
            this.Type = type;
        }
    }
}
