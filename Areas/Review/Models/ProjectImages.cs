using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using IMRepo.Models.Domain;

namespace IMRepo.Areas.Review.Models
{
    [DisplayName("Imagen")]
    public class ProjectImages : ProjectImage
    {

        // View fields for file upload (input type="file"
        [NotMapped]
        public List<IFormFile>? FilesInput { get; set; }

    }
}
