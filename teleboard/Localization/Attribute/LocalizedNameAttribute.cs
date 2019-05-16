using System.ComponentModel;
using System.Runtime.CompilerServices;
using Teleboard.Localization.ExtensionMethod;

namespace Teleboard.Localization.Attribute
{
    public class LocalizedNameAttribute : DisplayNameAttribute
    {
        public LocalizedNameAttribute([CallerMemberName] string propertyName = null)
            : base(propertyName)
        {
        }

        public LocalizedNameAttribute(string displayNameValue, [CallerMemberName] string propertyName = null)
            : base(displayNameValue)
        {
        }

        public override string DisplayName
        {
            get
            {
                return DisplayNameValue.Localize();
            }
        }
    }
}
