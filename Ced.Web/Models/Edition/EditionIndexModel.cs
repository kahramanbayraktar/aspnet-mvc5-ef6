using System.Collections.Generic;

namespace Ced.Web.Models.Edition
{
    public class EditionIndexModel : BaseModel
    {
        public int? EventId { get; set; }
        
        public string EventName { get; set; }

        public bool IsPrimaryDirector { get; set; }
        
        public List<EditionListModel> Editions { get; set; }
    }
}