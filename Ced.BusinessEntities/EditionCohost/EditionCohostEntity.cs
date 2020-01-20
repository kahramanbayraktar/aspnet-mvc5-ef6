namespace Ced.BusinessEntities
{
    public class EditionCohostEntity
    {
        public int EditionCohostId { get; set; }

        public int FirstEditionId { get; set; }

        public int SecondEditionId { get; set; }


        public EditionEntity FirstEdition { get; set; }

        public EditionEntity SecondEdition { get; set; }
    }
}
