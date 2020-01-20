using Ced.BusinessEntities;
using Ced.Data.Models;
using System;
using System.Collections.Generic;

namespace Ced.BusinessServices
{
    public interface ITaskServices
    {
        void UpdateEventEditionFromStagingDb(EditionEntity existingEdition, DWStaging.Edition stagingEdition, DateTime eventStartDate, DateTime eventEndDate);

        void CreateEventEditionFromStagingDb(DWStaging.Edition stagingEdition, DateTime eventStartDate, DateTime eventEndDate);

        bool UpdateEventsFromKentico();

        void UpdateEventDirectors();

        IList<Vw_EventInformations> GetEditionsWithMissingImages();

        void ResetEditionStartDateDifferences();
    }
}
