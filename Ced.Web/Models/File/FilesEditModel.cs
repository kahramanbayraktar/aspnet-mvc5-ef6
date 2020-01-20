using System.Collections.Generic;
using Ced.BusinessEntities;
using Ced.Web.Models.Edition;

namespace Ced.Web.Models.File
{
    public class FilesEditModel : EditionBaseModel
    {
        public int EntityId { get; set; }
        
        public EntityType EntityType { get; set; }

        public EditionFileType EditionFileType { get; set; }

        public IList<FileEntity> Files { get; set; }
    }
}