using AutoMapper;
using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Ced.BusinessServices
{
    public class NotificationServices : INotificationServices
    {
        private readonly UnitOfWork _unitOfWork;

        public NotificationServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public NotificationEntity GetNotification(int notificationId)
        {
            if (notificationId > 0)
            {
                var notification = _unitOfWork.NotificationRepository.GetById(notificationId);
                if (notification != null)
                {
                    var notificationModel = Mapper.Map<Notification, NotificationEntity>(notification);
                    return notificationModel;
                }
            }

            return null;
        }

        public IList<NotificationEntity> GetNotificationsByRecipient(string recipient, int? count = null, bool? displayed = null)
        {
            var context = _unitOfWork.NotificationRepository.Context;

            //var notifications = (from n in context.Notifications
            //    join ed in context.Editions on n.EditionId equals ed.EditionId
            //    join ev in context.Events on ed.EventId equals ev.EventId
            //    join edr in context.EventDirectors on ev.EventId equals edr.EventId
            //    where edr.DirectorEmail == recipient
            //          && edr.IsPrimary == true
            //          && (!displayed.HasValue || n.Displayed == displayed.Value)
            //    select n)
            //    .Distinct()
            //    .OrderByDescending(n => n.CreatedOn)
            //    .Take(count ?? int.MaxValue)
            //    .ToList();

            var notifications = (from n in context.Notifications
                    where n.ReceiverEmail.ToLower() == recipient.ToLower()
                          && (!displayed.HasValue || n.Displayed == displayed.Value)
                    select n)
                .OrderByDescending(n => n.CreatedOn)
                .Take(count ?? int.MaxValue)
                .ToList();

            if (notifications.Any())
            {
                var notificationsModel = Mapper.Map<List<Notification>, List<NotificationEntity>>(notifications);
                return notificationsModel;
            }

            return new List<NotificationEntity>();
        }

        public IList<NotificationEntity> GetNotificationsByEdition(int editionId)
        {
            var query = _unitOfWork.NotificationRepository.GetManyQueryable(x => x.EditionId == editionId);
            
            var notifications = query.ToList();

            if (notifications.Any())
            {
                var notificationsModel = Mapper.Map<List<Notification>, List<NotificationEntity>>(notifications);
                return notificationsModel;
            }
            return new List<NotificationEntity>();
        }

        public IList<NotificationEntity> GetNotifications(string recipient, NotificationType[] notificationTypes, int? lastXDays)
        {
            var now = DateTime.Now.Date;

            var query = _unitOfWork.NotificationRepository.GetManyQueryable(x => x.ReceiverEmail.ToLower() == recipient.ToLower());

            if (notificationTypes?.Length > 0)
            {
                var notifTypeIds = notificationTypes.Select(x => x.GetHashCode());
                query = query.Where(x => notifTypeIds.Contains(x.NotificationType));
            }

            if (lastXDays > 0)
                query = query.Where(x => DbFunctions.DiffDays(DbFunctions.TruncateTime(x.CreatedOn), now).Value <= lastXDays);

            var notifications = query.OrderByDescending(x => x.CreatedOn).ToList();

            if (notifications.Any())
            {
                var notificationsModel = Mapper.Map<List<Notification>, List<NotificationEntity>>(notifications);
                return notificationsModel;
            }
            return new List<NotificationEntity>();
        }

        public int GetNotificationCount(string recipient, bool? displayed = null)
        {
            var context = _unitOfWork.NotificationRepository.Context;

            //var count = (from n in context.Notifications
            //        join ed in context.Editions on n.EditionId equals ed.EditionId
            //        join ev in context.Events on ed.EventId equals ev.EventId
            //        join edr in context.EventDirectors on ev.EventId equals edr.EventId
            //        where edr.DirectorEmail == recipient
            //              && edr.IsPrimary == true
            //              && (!displayed.HasValue || n.Displayed == displayed.Value)
            //        select n)
            //    .Distinct()
            //    .Count();

            var count = (from n in context.Notifications
                    where n.ReceiverEmail.ToLower() == recipient.ToLower()
                          && (!displayed.HasValue || n.Displayed == displayed.Value)
                    select n)
                .Count();

            return count;
        }

        public NotificationEntity CreateNotification(NotificationEntity notificationEntity, int userId)
        {
            var notification = Mapper.Map<NotificationEntity, Notification>(notificationEntity);
            if (notification.CreatedOn <= DateTime.MinValue)
                notification.CreatedOn = DateTime.Now;

            _unitOfWork.NotificationRepository.Insert(notification);
            _unitOfWork.Save();

            var notfEntity = Mapper.Map<Notification, NotificationEntity>(notification);
            return notfEntity;
        }

        public void CreateNotifications(IList<NotificationEntity> notifications, int userId)
        {
            using (var scope = new TransactionScope())
            {
                foreach (var notification in notifications)
                {
                    CreateNotification(notification, userId);
                }
                scope.Complete();
            }
        }

        public void DisableNotifications(string recipient)
        {
            var context = _unitOfWork.NotificationRepository.Context;
            //var notifications = (from n in context.Notifications
            //    join ed in context.Editions on n.EditionId equals ed.EditionId
            //    join ev in context.Events on ed.EventId equals ev.EventId
            //    join edr in context.EventDirectors on ev.EventId equals edr.EventId
            //    where edr.DirectorEmail == recipient
            //          && n.Displayed == false
            //    select n).ToList(); // TODO: Distinct?

            var notifications = (from n in context.Notifications
                where n.ReceiverEmail.ToLower() == recipient.ToLower()
                      && n.Displayed == false
                select n).ToList();

            using (var scope = new TransactionScope())
            {
                foreach (var notification in notifications)
                {
                    notification.Displayed = true;
                    _unitOfWork.NotificationRepository.Update(notification);
                    _unitOfWork.Save();
                }
                scope.Complete();
            }
        }

        public void DeleteNotification(int notificationId)
        {
            if (notificationId > 0)
            {
                var exists = _unitOfWork.NotificationRepository.Exists(notificationId);
                if (exists)
                {
                    _unitOfWork.NotificationRepository.Delete(notificationId);
                    _unitOfWork.Save();
                }
            }
        }
    }
}
