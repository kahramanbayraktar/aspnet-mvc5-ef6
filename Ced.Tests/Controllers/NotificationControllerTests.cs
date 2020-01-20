using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using CedTests.Extensions;
using Ced.Utility;
using Ced.Web;
using Ced.Web.Controllers;
using Ced.Web.Helpers;
using Ced.Web.Models.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CedTests.Controllers
{
    [TestClass]
    public class NotificationControllerTests
    {
        private NotificationController _sut;
        private CedUser _cedUser;

        // AUTH
        private Mock<IUserServices> _mockAuthUserServices;
        private Mock<IRoleServices> _mockRoleServices;
        private Mock<IApplicationServices> _mockApplicationServices;
        private Mock<IIndustryServices> _mockIndustryServices;
        private Mock<IRegionServices> _mockRegionServices;

        private Mock<IEventServices> _mockEventServices;
        private Mock<IEventDirectorServices> _mockEventDirectorServices;
        private Mock<ILogServices> _mockLogServices;
        private Mock<INotificationServices> _mockNotificationServices;
        private Mock<IUserServices> _mockUserServices;
        private Mock<IUserRoleServices> _mockUserRoleServices;
        private Mock<IInAppNotificationHelper> _mockInAppNotificationHelper;

        private Mock<HttpContextBase> _mockHttpContextBase;

        [TestInitialize]
        public void TestInitialize()
        {
            _cedUser = new CedUser(new UserEntity { Email = "user1@company.com", }, null);

            _mockAuthUserServices = new Mock<IUserServices>();
            _mockRoleServices = new Mock<IRoleServices>();
            _mockApplicationServices = new Mock<IApplicationServices>();
            _mockIndustryServices = new Mock<IIndustryServices>();
            _mockRegionServices = new Mock<IRegionServices>();

            _mockEventServices = new Mock<IEventServices>();
            _mockEventDirectorServices = new Mock<IEventDirectorServices>();
            _mockLogServices = new Mock<ILogServices>();
            _mockNotificationServices = new Mock<INotificationServices>();
            _mockUserServices = new Mock<IUserServices>();
            _mockUserRoleServices = new Mock<IUserRoleServices>();
            _mockInAppNotificationHelper = new Mock<IInAppNotificationHelper>();

            _mockHttpContextBase = new Mock<HttpContextBase>();

            _mockNotificationServices
                .Setup(x => x.GetNotifications(It.IsAny<string>(), It.IsAny<NotificationType[]>(), It.IsAny<int?>()))
                .Returns(new List<NotificationEntity>());

            _sut = SetUpController();

            AutoMapperConfig.Register();
        }

        [TestMethod]
        public void Index_ValidRequest_ReturnsView()
        {
            // Arrange

            // Act
            var result = _sut.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(NotificationListModel));
        }

        [TestMethod]
        public void _SearchNotifications_ValidRequest_ReturnsView()
        {
            // Arrange
            var notificationTypes = new List<NotificationType>
            {
                NotificationType.GeneralInfoCompleteness,
                NotificationType.PostShowMetricsInfoCompleteness
            }.ToArray();

            // Act
            var result = _sut._SearchNotifications(notificationTypes, 5);

            // Assert
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.IsInstanceOfType(result.Model, typeof(IList<NotificationListItemModel>));
            Assert.AreEqual(result.ViewName, "_List");
        }

        [TestMethod]
        public void _ReadAllNotifications_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            _mockNotificationServices
                .Setup(x => x.DisableNotifications(It.IsAny<string>()));

            // Act
            var result = _sut._ReadAllNotifications();

            // Assert
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.IsTrue(result.GetValue<bool>("success"));
        }

        [TestMethod]
        public void _GetNotifications_WhenValid_ReturnsTrue()
        {
            // Arrange
            _mockNotificationServices.Setup(x =>
                x.GetNotificationsByRecipient(_cedUser.CurrentUser.Email, It.IsAny<int>(), It.IsAny<bool>()));

            _mockInAppNotificationHelper
                .Setup(x => x.GetNotificationViewModelItems(It.IsAny<IList<NotificationEntity>>()))
                .Returns(It.IsAny<IList<NotificationListItemModel>>());

            _mockNotificationServices.Setup(x =>
                x.GetNotificationCount(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(3);

            _sut.ViewEngineResultFunc = delegate { return "Something"; };

            // Act
            var result = _sut._GetNotifications();

            // Assert
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.AreEqual("Something", result.GetValue<string>("data"));
            Assert.IsTrue(result.GetValue<int>("count") == 3);
        }

        private NotificationController SetUpController()
        {
            var routes = new RouteCollection();
            var controller = new NotificationController(
                _mockAuthUserServices.Object,
                _mockRoleServices.Object,
                _mockApplicationServices.Object,
                _mockIndustryServices.Object,
                _mockRegionServices.Object,
                _mockEventServices.Object,
                _mockEventDirectorServices.Object,
                _mockLogServices.Object,
                _mockNotificationServices.Object,
                _mockUserServices.Object,
                _mockUserRoleServices.Object,
                _mockInAppNotificationHelper.Object)
            {
                CurrentCedUser = _cedUser
            };

            var routeData = new RouteData();
            routeData.Values.Add("controller", "NotificationController");
            controller.ControllerContext = new ControllerContext(_mockHttpContextBase.Object, routeData, controller);
            controller.Url = new UrlHelper(new RequestContext(_mockHttpContextBase.Object, routeData), routes);

            return controller;
        }
    }
}