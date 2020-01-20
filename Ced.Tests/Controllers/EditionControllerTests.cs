using Ced.BusinessEntities;
using Ced.BusinessEntities.Auth;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Utility.Edition;
using Ced.Web;
using Ced.Web.Controllers;
using Ced.Web.Models;
using Ced.Web.Models.Edition;
using ITE.Utility.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Web.Mvc;
using Ced.BusinessServices;

namespace CedTests.Controllers
{
    [TestClass]
    public class EditionControllerTests
    {
        private const int CedAppId = 2;

        private EditionController _sut;
        private CedUser _cedUser;

        private Mock<IUserServices> _mockAuthUserServices;
        private Mock<IRoleServices> _mockRoleServices;
        private Mock<IApplicationServices> _mockApplicationServices;
        private Mock<IIndustryServices> _mockIndustryServices;
        private Mock<IRegionServices> _mockRegionServices;

        private Mock<ICountryServices> _mockCountryServices;
        private Mock<IEditionServices> _mockEditionServices;
        private Mock<IEditionCohostServices> _mockEditionCohostServices;
        private Mock<IEditionCountryServices> _mockEditionCountryServices;
        private Mock<IEditionKeyVisitorServices> _mockEditionKeyVisitorServices;
        private Mock<IEditionTranslationServices> _mockEditionTranslationServices;
        private Mock<IEditionTranslationSocialMediaServices> _mockEditionTranslationSocialMediaServices;
        private Mock<IEditionVisitorServices> _mockEditionVisitorServices;
        private Mock<IEventServices> _mockEventServices;
        private Mock<IEventDirectorServices> _mockEventDirectorServices;
        private Mock<IFileServices> _mockFileServices;
        private Mock<IKeyVisitorServices> _mockKeyVisitorServices;
        private Mock<ILogServices> _mockLogServices;
        private Mock<INotificationServices> _mockNotificationServices;
        private Mock<ISubscriptionServices> _mockSubscriptionServices;
        private Mock<IUserServices> _mockUserServices;
        private Mock<IUserRoleServices> _mockUserRoleServices;
        private Mock<IEditionHelper> _mockEditionHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            _cedUser = new CedUser(new UserEntity { Email = "user1@domain.com" }, new List<RoleEntity>());

            _mockAuthUserServices = new Mock<IUserServices>();
            _mockRoleServices = new Mock<IRoleServices>();
            _mockApplicationServices = new Mock<IApplicationServices>();
            _mockIndustryServices = new Mock<IIndustryServices>();
            _mockRegionServices = new Mock<IRegionServices>();

            _mockCountryServices = new Mock<ICountryServices>();
            _mockEditionServices = new Mock<IEditionServices>();
            _mockEditionCohostServices = new Mock<IEditionCohostServices>();
            _mockEditionCountryServices = new Mock<IEditionCountryServices>();
            _mockEditionKeyVisitorServices = new Mock<IEditionKeyVisitorServices>();
            _mockEditionTranslationServices = new Mock<IEditionTranslationServices>();
            _mockEditionTranslationSocialMediaServices = new Mock<IEditionTranslationSocialMediaServices>();
            _mockEditionVisitorServices = new Mock<IEditionVisitorServices>();
            _mockEventServices = new Mock<IEventServices>();
            _mockEventDirectorServices = new Mock<IEventDirectorServices>();
            _mockFileServices = new Mock<IFileServices>();
            _mockKeyVisitorServices = new Mock<IKeyVisitorServices>();
            _mockLogServices = new Mock<ILogServices>();
            _mockNotificationServices = new Mock<INotificationServices>();
            _mockSubscriptionServices = new Mock<ISubscriptionServices>();
            _mockUserServices = new Mock<IUserServices>();
            _mockUserRoleServices = new Mock<IUserRoleServices>();
            _mockEditionHelper = new Mock<IEditionHelper>();

            _sut = SetUpController();

            AutoMapperConfig.Register();
        }

        [TestMethod]
        public void Index_ValidRequest_ReturnsEditionIndexModel()
        {
            // Arrange
            int? eventId = null;

            _mockEditionServices.Setup(x =>
                    x.GetEditions(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<int?>(),
                        It.IsAny<EditionStatusType[]>(), It.IsAny<string[]>(), It.IsAny<string[]>()))
                .Returns(() => new List<EditionEntityLight>());

            _mockEventDirectorServices
                .Setup(x => x.IsPrimaryDirector(_cedUser.CurrentUser.Email, eventId, CedAppId));

            // Act
            var result = _sut.Index(null, null);

            // Assert
            Assert.IsInstanceOfType(result.Model, typeof(EditionIndexModel));
            Assert.AreEqual(0, ((EditionIndexModel)result.Model).Editions.Count);
        }

        [TestMethod]
        public void Index_EditionStatusTypeNeedsApprovalAndUserNotHavePermission_ReturnsUnauthorized()
        {
            // Arrange
            var status = EditionStatusType.WaitingForApproval.ToString();

            // Act
            var result = _sut.Index(null, status);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(result.ViewName, "Unauthorized");
            Assert.IsInstanceOfType(result.Model, typeof(ErrorModel));
            Assert.AreEqual(((ErrorModel)result.Model).Message, Constants.NotAuthorizedToEditionsOfWaitingForApproval);
        }

        [TestMethod]
        public void Index_EditionStatusTypeNeedsApprovalAndUserHasPermission_ReturnsEditionIndexModel()
        {
            // Arrange
            var status = EditionStatusType.WaitingForApproval.ToString();

            _sut.CurrentCedUser.Roles.Add(new RoleEntity { ApplicationId = CedAppId, Name = "Super Admin" });

            // Act
            var result = _sut.Index(null, status);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(result.ViewName, "Unauthorized");
            Assert.IsInstanceOfType(result.Model, typeof(ErrorModel));
            Assert.AreEqual(((ErrorModel)result.Model).Message, Constants.NotAuthorizedToEditionsOfWaitingForApproval);
        }

        [TestMethod]
        public void Index_EventNotFound_ReturnsNotFound()
        {
            // Arrange
            var eventId = 0;

            // Act
            var result = _sut.Index(eventId, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(result.ViewName, "NotFound");
            Assert.IsInstanceOfType(result.Model, typeof(ErrorModel));
            Assert.AreEqual(((ErrorModel)result.Model).Message, Constants.NoEventFoundWithThisId);
        }

        [TestMethod]
        public void Details_EventNotFound_ReturnsNotFound()
        {
            // Arrange
            var eventId = 0;

            // Act
            var result = _sut.Index(eventId, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(result.ViewName, "NotFound");
            Assert.IsInstanceOfType(result.Model, typeof(ErrorModel));
            Assert.AreEqual(((ErrorModel)result.Model).Message, Constants.NoEventFoundWithThisId);
        }

        [TestMethod]
        public void Details_EditionStatusTypeNotPublishedNorApproved_ReturnsNotFound()
        {
            // Arrange
            _mockEditionServices
                .Setup(x => x.GetEditionById(It.IsAny<int>(), null)).
                Returns(new EditionEntity { Status = EditionStatusType.Draft });

            // Act
            var result = _sut.Details(1, null);

            // Assert
            var viewResult = ((ViewResult)result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(viewResult.ViewName, "NotFound");
            Assert.IsInstanceOfType(viewResult.Model, typeof(ErrorModel));
            //Assert.AreEqual(((ErrorModel)viewResult.Model).Message, "An event must have already been published or approved to be displayed.");
        }

        [TestMethod]
        public void Details_ValidRequest_ReturnsEditionDetailsModel()
        {
            // Arrange
            _mockEditionServices
                .Setup(x => x.GetEditionById(It.IsAny<int>(), It.IsAny<string[]>())).
                Returns(new EditionEntity { Status = EditionStatusType.Published, Event = new EventEntity { EventTypeCode = EventType.Conference.GetDescription() } });

            _mockEditionTranslationServices
                .Setup(x => x.GetEditionTranslation(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new EditionTranslationEntity());

            _mockEditionCountryServices
                .Setup(x => x.GetEditionCountriesByEdition(It.IsAny<int>(), It.IsAny<EditionCountryRelationType>()))
                .Returns(new List<EditionCountryEntity>());

            _mockEditionVisitorServices
                .Setup(x => x.GetEditionVisitors(It.IsAny<int>()))
                .Returns(new List<EditionVisitorEntity>());

            _mockEditionTranslationSocialMediaServices
                .Setup(x => x.GetByEdition(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new List<EditionTranslationSocialMediaEntity>());

            _mockEventDirectorServices
                .Setup(x => x.GetPrimaryDirectors(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<EventDirectorEntity>());

            // Act
            var result = _sut.Details(1, null);

            // Assert
            var viewResult = ((ViewResult)result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("", viewResult.ViewName);
            Assert.IsInstanceOfType(viewResult.Model, typeof(EditionDetailsModel));
        }

        private EditionController SetUpController()
        {
            //var routes = new RouteCollection();
            var controller = new EditionController(
                _mockAuthUserServices.Object,
                _mockRoleServices.Object,
                _mockApplicationServices.Object,
                _mockIndustryServices.Object,
                _mockRegionServices.Object,
                _mockCountryServices.Object,
                _mockEditionServices.Object,
                _mockEditionCohostServices.Object,
                _mockEditionCountryServices.Object,
                _mockEditionKeyVisitorServices.Object,
                _mockEditionTranslationServices.Object,
                _mockEditionTranslationSocialMediaServices.Object,
                _mockEditionVisitorServices.Object,
                _mockEventServices.Object,
                _mockEventDirectorServices.Object,
                _mockFileServices.Object,
                _mockKeyVisitorServices.Object,
                _mockLogServices.Object,
                _mockNotificationServices.Object,
                _mockSubscriptionServices.Object,
                _mockUserServices.Object,
                _mockUserRoleServices.Object,
                _mockEditionHelper.Object)
            {
                CurrentCedUser = _cedUser
            };

            //var routeData = new RouteData();
            //routeData.Values.Add("controller", "NotificationController");
            //controller.ControllerContext = new ControllerContext(_mockHttpContextBase.Object, routeData, controller);
            //controller.Url = new UrlHelper(new RequestContext(_mockHttpContextBase.Object, routeData), routes);
            //controller.CurrentCedUser = _cedUser;

            return controller;
        }
    }
}
