using System.ComponentModel.DataAnnotations;
using Teleboard.Localization.ExtensionMethod;

namespace Teleboard.Validation.Attribute
{
    public class StringLengthRangeAttribute : System.ComponentModel.DataAnnotations.StringLengthAttribute
    {
        public StringLengthRangeAttribute(int minimumLength, int maximumLength, string errorMessageResourceName = "InvalidStringLength")
            : base(maximumLength)
        {
            this.MinimumLength = minimumLength;
            this.ErrorMessageResourceName = errorMessageResourceName;
            this.ErrorMessageResourceType = typeof(Teleboard.Localization.Resources);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("MsgStringLengthRange".Localize(), name, MinimumLength, MaximumLength );
        }
    }
}
