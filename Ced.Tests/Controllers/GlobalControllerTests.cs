using Ced.BusinessEntities;
using Ced.BusinessEntities.Auth;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using CedTests.Extensions;
using Ced.Utility;
using Ced.Utility.Email;
using Ced.Web;
using Ced.Web.Controllers;
using Ced.Web.Models.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ICountryServices = Ced.BusinessServices.ICountryServices;

namespace CedTests.Controllers
{
    [TestClass]
    public class GlobalControllerTests
    {
        private GlobalController _sut;
        private CedUser _cedUser;

        // AUTH
        private Mock<IUserServices> _mockAuthUserServices;
        private Mock<IRoleServices> _mockRoleServices;
        private Mock<IApplicationServices> _mockApplicationServices;
        private Mock<IIndustryServices> _mockIndustryServices;
        private Mock<IRegionServices> _mockRegionServices;

        private Mock<ICountryServices> _mockCountryServices;
        private Mock<IEditionServices> _mockEditionServices;
        private Mock<IEditionCohostServices> _mockEditionCoHostServices;
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

        [TestInitialize]
        public void TestInitialize()
        {
            _cedUser = new CedUser(new UserEntity {Email = "user1@domain.com"}, new List<RoleEntity>());

            _mockAuthUserServices = new Mock<IUserServices>();
            _mockRoleServices = new Mock<IRoleServices>();
            _mockApplicationServices = new Mock<IApplicationServices>();
            _mockIndustryServices = new Mock<IIndustryServices>();
            _mockRegionServices = new Mock<IRegionServices>();

            _mockCountryServices = new Mock<ICountryServices>();
            _mockEditionServices = new Mock<IEditionServices>();
            _mockEditionCoHostServices = new Mock<IEditionCohostServices>();
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

            _sut = SetUpController();

            AutoMapperConfig.Register();
        }

        [TestMethod]
        public void _Navigation_CurrentUserNotNull_ReturnsUserViewModelWithUser()
        {
            // Arrange
            var expectedCount = 5;
            var expectedUrl = "SomeUrl";

            _mockEditionServices.Setup(x => x.GetEditionsCount(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(),
                    It.IsAny<int?>(), It.IsAny<EditionStatusType[]>(), It.IsAny<string[]>(), It.IsAny<string[]>()))
                .Returns(expectedCount);

            _mockUserServices.Setup(x => x.GetProfilePictureUrl(It.IsAny<int>()))
                .Returns(expectedUrl);

            SetApprovers();

            // Act
            var result = _sut._Navigation();

            // Assert
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.AreEqual(result.ViewName, "_Navigation");
            Assert.IsInstanceOfType(result.Model, typeof(UserViewModel));
            var userViewModel = (UserViewModel) result.Model;
            Assert.IsNotNull(((UserViewModel) result.Model).CurrentUser);
            Assert.AreEqual(expectedUrl, userViewModel.ProfilePictureUrl);
        }

        [TestMethod]
        public void _Navigation_CurrentUserNull_ReturnsUserViewModelWithoutUser()
        {
            // Arrange
            _sut.CurrentCedUser = null;

            SetApprovers();

            // Act
            var result = _sut._Navigation();

            // Assert
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.AreEqual(result.ViewName, "_Navigation");
            Assert.IsInstanceOfType(result.Model, typeof(UserViewModel));
            var userViewModel = (UserViewModel) result.Model;
            Assert.IsNull(userViewModel.CurrentUser);
        }

        //[TestMethod]
        //public void _RequestHelp_InvalidRequest_ValidatesCorrectly()
        //{
        //    // Arrange
        //    var context = new ValidationContext(_sut, null, null);
        //    var results = new List<ValidationResult>();

        //    // Act
        //    var result = Validator.TryValidateObject(_sut, context, results, true);

        //    // Assert
        //    Assert.AreEqual(false, false);
        //}

        [TestMethod]
        public void LogEmail_RecipientsNotExist_ReturnsFalse()
        {
            // Arrange
            _mockEditionServices.Setup(x => x.GetEditionById(It.IsAny<int>(), It.IsAny<string[]>()))
                .Returns(new EditionEntity());

            _sut.CreateEmailLogFunc = delegate { return new LogEntity(); };

            // Act
            var result = _sut.LogEmail(It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ActionType>());

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LogEmail_RecipientsExist_ReturnsTrue()
        {
            // Arrange
            _mockEditionServices.Setup(x => x.GetEditionById(It.IsAny<int>(), It.IsAny<string[]>()))
                .Returns(new EditionEntity());

            _sut.CreateEmailLogFunc = delegate { return new LogEntity(); };

            // Act
            var result = _sut.LogEmail(It.IsAny<int?>(), _cedUser.CurrentUser.Email, It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ActionType>());

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void _RequestHelp_Invalid_ReturnsJsonRequest()
        {
            // Arrange
            var model = new HelpRequestModel();
            _sut.EmailHelper = new Mock<IEmailHelper>().Object;

            // Act
            var result = _sut._RequestHelp(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var jsonResult = (JsonResult) result;
            Assert.AreEqual(true, jsonResult.GetValue<bool>("success"));
        }

        [TestMethod]
        public void SetUserSso_Valid_SetsCurrentUser()
        {
            // Arrange
            var httpContext = GetHttpContextMock(true);
            var controllerContext = new ControllerContext(httpContext.Object, new RouteData(), _sut);
            _sut.ControllerContext = controllerContext;

            _mockUserServices.Setup(x => x.GetUser(It.IsAny<string>())).Returns(_cedUser.CurrentUser);
            _mockUserRoleServices.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int?>())).Returns(new List<UserRoleEntity>());

            // Act
            _sut.SetUserSso();

            // Assert
            _mockUserServices.Verify(x => x.GetUser(It.IsAny<string>()), Times.Once);
            _mockUserRoleServices.Verify(x => x.Get(It.IsAny<int>(), It.IsAny<int?>()), Times.Once);
            Assert.AreEqual(_cedUser.CurrentUser.Email, _sut.CurrentCedUser.CurrentUser.Email);
        }

        [TestMethod]
        public void SetRecentlyViewedEvents_CurrentUserExists_ThrowsNoException()
        {
            // Arrange
            _mockEventServices.Setup(x => x.GetLastViewedEvents(It.IsAny<int>(), It.IsAny<int?>()))
                .Returns(new List<EventEditionCustomType>
                {
                    new EventEditionCustomType
                    {
                        EditionId = 1,
                        EditionName = "Edition 1",
                        EventId = 1,
                        MasterName = "Event Master Name 1",
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(2),
                        Logo = "{ 'WebLogoFileName': 'image_1.png' }"
                    }
                });

            // Act
            _sut.SetRecentlyViewedEvents();

            // Assert
        }

        private Mock<HttpContextBase> GetHttpContextMock(bool isLoggedIn)
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var principal = GetPrincipalMock(isLoggedIn);

            context.SetupGet(c => c.Request).Returns(request.Object);
            context.SetupGet(c => c.Response).Returns(response.Object);
            context.SetupGet(c => c.Session).Returns(session.Object);
            context.SetupGet(c => c.Server).Returns(server.Object);
            context.SetupGet(c => c.User).Returns(principal.Object);

            return context;
        }

        private Mock<ClaimsPrincipal> GetPrincipalMock(bool isLoggedIn)
        {
            var principal = new Mock<ClaimsPrincipal>();

            principal.SetupGet(p => p.Claims).Returns(new List<Claim>{ new Claim("preferred_username", _cedUser.CurrentUser.Email) });
            principal.SetupGet(p => p.Identity).Returns(GetIdentityMock(isLoggedIn).Object);
            principal.Setup(p => p.IsInRole(It.IsAny<string>())).Returns(false);

            return principal;
        }

        private Mock<IIdentity> GetIdentityMock(bool isLoggedIn)
        {
            var identity = new Mock<IIdentity>();

            identity.SetupGet(i => i.AuthenticationType).Returns(isLoggedIn ? "Mock Identity" : null);
            identity.SetupGet(i => i.IsAuthenticated).Returns(isLoggedIn);
            identity.SetupGet(i => i.Name).Returns(isLoggedIn ? "TestUser" : null);

            return identity;
        }

        private GlobalController SetUpController()
        {
            var controller = new GlobalController(
                _mockAuthUserServices.Object,
                _mockRoleServices.Object,
                _mockApplicationServices.Object,
                _mockIndustryServices.Object,
                _mockRegionServices.Object,
                _mockCountryServices.Object,
                _mockEditionServices.Object,
                _mockEditionCoHostServices.Object,
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
                _mockUserRoleServices.Object)
            {
                CurrentCedUser = _cedUser
            };

            return controller;
        }

        private void SetApprovers()
        {
            _mockUserServices.Setup(x => x.GetUsersByRole(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new List<UserEntity> { _cedUser.CurrentUser });
        }
    }
}
