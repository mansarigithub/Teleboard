using Teleboard.Localization.Attribute;

namespace Teleboard.PresentationModel.Model.ContentType
{
    public class ContentTypePM
    {
        public int Id { get; set; }

        [LocalizedName]
        public string Name { get; set; }
    }
}
