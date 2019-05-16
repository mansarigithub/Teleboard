using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Teleboard.Localization.Attribute;
using Teleboard.PresentationModel.Model.ContentType;
using Teleboard.Validation.Attribute;

namespace Teleboard.Models
{
    public class ContentUploadViewModel
    {
        [RequiredField]
        [LocalizedName("Tenant")]
        public int TenantId { get; set; }

        [RequiredField]
        [LocalizedName]
        [StringLengthRange(1, 200)]
        public string Description { get; set; }

        //[Required, FileExtensions(Extensions = ".png,.jpg,.jpeg,.mp4", ErrorMessage = "Incorrect file format")]
        [LocalizedName("File")]
        public HttpPostedFileBase FileStream { get; set; }

        public IEnumerable<ContentTypePM> SupportedContentTypes { get; set; }
    }
}