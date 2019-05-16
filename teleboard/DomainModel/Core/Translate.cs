using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.DomainModel.Core
{
    public class Translate
    {
        public int Id { get; set; }

        [RequiredField]
        [LocalizedName("Culture")]
        public string Culture { get; set; }

        [RequiredField]
        [LocalizedName("Name")]
        public string Name { get; set; }

        [RequiredField]
        [LocalizedName("Value")]
        public string Value { get; set; }

    }
}