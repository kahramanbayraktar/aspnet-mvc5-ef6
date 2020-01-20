using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface ISubscriptionServices
    {
        SubscriptionEntity GetSubscription(int editionId, string email);

        IList<SubscriptionEntity> GetSubscriptions(int editionId);

        int CreateSubscription(int editionId, string email, int userId);

        bool DeleteSubscription(int editionId, string email);
    }
}