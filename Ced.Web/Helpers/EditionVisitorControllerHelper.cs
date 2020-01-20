using Ced.BusinessEntities;
using Ced.BusinessServices;
using System.Collections.Generic;

namespace Ced.Web.Helpers
{
    public class EditionVisitorControllerHelper
    {
        private readonly IEditionVisitorServices _editionVisitorServices;

        public EditionVisitorControllerHelper(IEditionVisitorServices editionVisitorServices)
        {
            _editionVisitorServices = editionVisitorServices;
        }

        public IList<EditionVisitorEntity> GetEditionVisitors(EditionEntity edition)
        {
            var editionVisitors = _editionVisitorServices.GetEditionVisitors(edition.EditionId);

            if (editionVisitors == null || editionVisitors.Count == 0)
            {
                editionVisitors = new List<EditionVisitorEntity>();
                var dayCount = (edition.EndDate.GetValueOrDefault() - edition.StartDate.GetValueOrDefault()).Days + 1;
                for (var i = 0; i < dayCount; i++)
                {
                    editionVisitors.Add(new EditionVisitorEntity
                    {
                        EditionId = edition.EditionId,
                        DayNumber = (byte)(i + 1),
                        VisitorCount = 0,
                        RepeatVisitCount = 0,
                        OldVisitorCount = 0,
                        NewVisitorCount = 0,
                    });
                }
            }

            return editionVisitors;
        }
    }
}