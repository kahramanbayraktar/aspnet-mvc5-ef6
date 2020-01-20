namespace Ced.BusinessEntities
{
    public class EditionCountryEntity
    {
        public int EditionCountryId { get; set; }
        
        public int EditionId { get; set; }
        
        public string CountryCode { get; set; }

        public EditionCountryRelationType RelationType { get; set; }
    }
}