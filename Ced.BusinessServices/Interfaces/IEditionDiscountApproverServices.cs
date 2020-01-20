using Ced.BusinessEntities;
using System.Collections.Generic;

namespace Ced.BusinessServices
{
    public interface IEditionDiscountApproverServices
    {
        EditionDiscountApproverEntity GetById(int id);

        EditionDiscountApproverEntity Get(int editionId, string approvingUser);

        IList<EditionDiscountApproverEntity> GetByEdition(int editionId);

        int Create(EditionDiscountApproverEntity entity, int userId);

        bool Delete(int id);
    }
}