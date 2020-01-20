using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using Ced.IntegrationTests.Extensions;
using Ced.Utility.Edition;
using Ced.Web;
using Ced.Web.Controllers;
using Ced.Web.Helpers;
using Ced.Web.Models.Notification;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.IntegrationTests.Controllers
{
    [TestFixture]
    public class NotificationControllerTests
    {
        private readonly CedContext _context = new CedContext();

        // AUTH
        private IUserServices _authUserServices;
        private IRoleServices _roleServices;
        private IApplicationServices _applicationServices;
        private IIndustryServices _industryServices;
        private IRegionServices _regionServices;

        private IEventServices _eventServices;
        private IEventDirectorServices _eventDirectorServices;
        private ILogServices _logServices;
        private INotificationServices _notificationServices;
        private IUserServices _userServices;
        private IUserRoleServices _userRoleServices;

        private IEditionHelper _editionHelper;
        private IInAppNotificationHelper _inAppNotificationHelper;

        private NotificationController _controller;

        [SetUp]
        public void SetUp()
        {
            var unitOfWork = new UnitOfWork();

            _authUserServices = new UserServices(unitOfWork);
            _roleServices = new RoleServices(unitOfWork);
            _applicationServices = new ApplicationServices(unitOfWork);
            _industryServices = new IndustryServices(unitOfWork);
            _regionServices = new RegionServices(unitOfWork);

            _eventServices = new EventServices(unitOfWork);
            _eventDirectorServices = new EventDirectorServices(unitOfWork);
            _logServices = new LogServices(new UnitOfWork());
            _notificationServices = new NotificationServices(new UnitOfWork());
            _userServices = new UserServices(unitOfWork);
            _userRoleServices = new UserRoleServices(unitOfWork);

            _editionHelper = new EditionHelper();
            _inAppNotificationHelper = new InAppNotificationHelper(_editionHelper);

            _controller = new NotificationController(
                _authUserServices,
                _roleServices,
                _applicationServices,
                _industryServices,
                _regionServices,
                _eventServices,
                _eventDirectorServices,
                _logServices,
                _notificationServices,
                _userServices,
                _userRoleServices,
                _inAppNotificationHelper);
            _controller.SetDefaultUser();

            AutoMapperConfig.Register();
        }

        [Test, Isolated]
        public void Index_ValidRequest_ReturnsNotifications()
        {
            // Arrange
            var @event = new Event();
            @event.FillWithDefaultValues();
            @event = _context.Events.Add(@event);

            var edition = new Edition();
            edition.FillWithDefaultValues(@event);
            edition = _context.Editions.Add(edition);

            var notification = new Notification();
            notification.FillWithDefaultValues(edition);
            _context.Notifications.Add(notification);

            _context.SaveChanges();

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOf<NotificationListModel>(result.Model);
            Assert.AreEqual(1, ((NotificationListModel)result.Model).Notifications.Count);
        }

        [Test, Isolated]
        public void _SearchNotifications_MultiTypeOfNotificationsExist_ReturnsOnlySelectedTypeOfNotifications()
        {
            // Arrange
            var expectedNotificationType = NotificationType.GeneralInfoCompleteness;
            var unexpectedNotificationType = NotificationType.PostShowMetricsInfoCompleteness;

            var @event = new Event();
            @event.FillWithDefaultValues();
            @event = _context.Events.Add(@event);

            var edition = new Edition();
            edition.FillWithDefaultValues(@event);
            edition = _context.Editions.Add(edition);

            var notification = new Notification();
            notification.FillWithDefaultValues(edition);
            notification.NotificationType = (byte)expectedNotificationType.GetHashCode();
            notification.Displayed = false;
            _context.Notifications.Add(notification);
            notification = new Notification();
            notification.FillWithDefaultValues(edition);
            notification.NotificationType = (byte)unexpectedNotificationType.GetHashCode();
            notification.Displayed = false;
            _context.Notifications.Add(notification);

            _context.SaveChanges();

            // Act
            var result = _controller._SearchNotifications(new[] { expectedNotificationType }, null );

            // Assert
            Assert.IsInstanceOf<PartialViewResult>(result);
            Assert.AreEqual(true, ((IList<NotificationListItemModel>) result.Model).Any(x => x.Type == expectedNotificationType));
            Assert.AreEqual(true, ((IList<NotificationListItemModel>) result.Model).All(x => x.Type != unexpectedNotificationType));
        }

        [Test, Isolated]
        public void _ReadAllNotifications_OneNotificationExists_ReturnsZero()
        {
            // Arrange
            var @event = new Event();
            @event.FillWithDefaultValues();
            @event = _context.Events.Add(@event);

            var edition = new Edition();
            edition.FillWithDefaultValues(@event);
            edition = _context.Editions.Add(edition);

            var notification = new Notification();
            notification.FillWithDefaultValues(edition);
            notification.Displayed = false;
            _context.Notifications.Add(notification);

            _context.SaveChanges();

            // Act
            var result = _controller._ReadAllNotifications();

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            Assert.AreEqual(true, result.GetValue<bool>("success"));
        }

        [Test, Isolated]
        public void _GetNotifications_UserHasTwoNotifications_ReturnsTwoNotifications()
        {
            // Arrange
            var @event = new Event();
            @event.FillWithDefaultValues();
            @event = _context.Events.Add(@event);

            var edition = new Edition();
            edition.FillWithDefaultValues(@event);
            edition = _context.Editions.Add(edition);

            var notification = new Notification();
            notification.FillWithDefaultValues(edition);
            notification.Displayed = false;
            _context.Notifications.Add(notification);
            notification = new Notification();
            notification.FillWithDefaultValues(edition);
            notification.Displayed = false;
            _context.Notifications.Add(notification);

            _context.SaveChanges();

            _controller.ViewEngineResultFunc = delegate { return ""; };

            // Act
            var result = _controller._GetNotifications();

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            Assert.AreEqual(2, result.GetValue<int>("count"));
        }
    }
}
