using Ced.BusinessEntities;
using System.Collections.Generic;

namespace Ced.BusinessServices
{
    public interface IEditionPaymentScheduleServices
    {
        EditionPaymentScheduleEntity GetById(int id);

        EditionPaymentScheduleEntity Get(int editionId, string name);

        IList<EditionPaymentScheduleEntity> GetByEdition(int editionId);

        int Create(EditionPaymentScheduleEntity entity, int userId);

        bool Delete(int id);
    }
}