using Ced.BusinessEntities;

namespace Ced.Web.Models.Edition
{
    public class EditionEditImagesModel : EditionBaseModel
    {
        public string EditionName { get; set; }
        
        public EditionImageType EditionImageType { get; set; }
        
        public string ProductImageFileName { get; set; }
        
        public string MasterLogoFileName { get; set; }

        public string CrmLogoFileName { get; set; }

        public string IconFileName { get; set; }

        public string PromotedLogoFileName { get; set; }

        public string DetailsImageFileName { get; set; }
    }
}