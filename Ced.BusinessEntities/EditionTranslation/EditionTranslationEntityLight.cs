namespace Ced.BusinessEntities
{
    public class EditionTranslationEntityLight
    {
        public int EditionTranslationId { get; set; }

        public int EditionId { get; set; }

        public string LanguageCode { get; set; }

        public string BookStandUrl { get; set; }

        public string OnlineInvitationUrl { get; set; }

        public string WebLogoFileName { get; set; }

        public string VenueName { get; set; }

        //public string PeopleImageFileName { get; set; }

        //public string ProductImageFileName { get; set; }

        //public string MasterLogoFileName { get; set; }

        //public string IconFileName { get; set; }
    }
}
