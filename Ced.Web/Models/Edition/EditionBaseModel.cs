using Ced.BusinessEntities;

namespace Ced.Web.Models.Edition
{
    public class EditionBaseModel : BaseModel
    {
        public int EditionId { get; set; }
        
        public int EditionTranslationId { get; set; }

        public string LanguageCode { get; set; }

        public int EventId { get; set; }

        public string EventMasterName { get; set; }

        public string EventActivity { get; set; }

        public EventType EventType { get; set; }

        public bool IsAlive { get; set; }
        
        public bool IsCancelled { get; set; }
        
        public bool IsEditableForImages { get; set; }
    }
}