using AutoMapper;
using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices
{
    public class SubscriptionServices : ISubscriptionServices
    {
        private readonly UnitOfWork _unitOfWork;
        
        public SubscriptionServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public SubscriptionEntity GetSubscription(int editionId, string email)
        {
            email = email.Trim().ToLower();
            var query = _unitOfWork.SubscriptionRepository.GetManyQueryable(x => x.EditionId == editionId && x.UserEmail == email);

            var subscription = query.SingleOrDefault();
            if (subscription != null)
            {
                var subscriptionDto = Mapper.Map<Subscription, SubscriptionEntity>(subscription);
                return subscriptionDto;
            }
            return null;
        }

        public IList<SubscriptionEntity> GetSubscriptions(int editionId)
        {
            var query = _unitOfWork.SubscriptionRepository.GetManyQueryable(x => x.EditionId == editionId);

            var subscriptions = query.ToList();
            if (subscriptions.Any())
            {
                var subscriptionDtos = Mapper.Map<IList<Subscription>, IList<SubscriptionEntity>>(subscriptions);
                return subscriptionDtos;
            }
            return new List<SubscriptionEntity>();
        }

        public int CreateSubscription(int editionId, string email, int userId)
        {
            var existing = GetSubscription(editionId, email);

            if (existing != null)
                return 0;

            var subscription = new Subscription
            {
                EditionId = editionId,
                UserEmail = email,
                CreatedOn = DateTime.Now,
                CreatedBy = userId
            };

            _unitOfWork.SubscriptionRepository.Insert(subscription);
            _unitOfWork.Save();
            return subscription.SubscriptionId;
        }

        public bool DeleteSubscription(int editionId, string email)
        {
            email = email.Trim().ToLower();
            var query = _unitOfWork.SubscriptionRepository.GetManyQueryable(x => x.UserEmail == email && x.EditionId == editionId);

            var subscription = query.SingleOrDefault();

            if (subscription != null)
            {
                _unitOfWork.SubscriptionRepository.Delete(subscription);
                _unitOfWork.Save();

                return true;
            }
            return false;
        }
    }
}
