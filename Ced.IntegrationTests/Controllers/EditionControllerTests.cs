using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using Ced.IntegrationTests.Extensions;
using Ced.Utility.Edition;
using Ced.Web;
using Ced.Web.Controllers;
using Ced.Web.Models.Edition;
using NUnit.Framework;
using System.Web.Mvc;
using Assert = NUnit.Framework.Assert;
using CountryServices = Ced.BusinessServices.CountryServices;
using ICountryServices = Ced.BusinessServices.ICountryServices;

namespace Ced.IntegrationTests.Controllers
{
    [TestFixture]
    public class EditionControllerTests
    {
        private readonly CedContext _context = new CedContext();

        private IUserServices _authUserServices;
        private IRoleServices _roleServices;
        private IApplicationServices _applicationServices;
        private IIndustryServices _industryServices;
        private IRegionServices _regionServices;

        private ICountryServices _countryServices;
        private IEditionServices _editionServices;
        private IEditionCohostServices _editionCohostServices;
        private IEditionCountryServices _editionCountryServices;
        private IEditionKeyVisitorServices _editionKeyVisitorServices;
        private IEditionTranslationServices _editionTranslationServices;
        private IEditionTranslationSocialMediaServices _editionTranslationSocialMediaServices;
        private IEditionVisitorServices _editionVisitorServices;
        private IEventServices _eventServices;
        private IEventDirectorServices _eventDirectorServices;
        private IFileServices _fileServices;
        private IKeyVisitorServices _keyVisitorServices;
        private ILogServices _logServices;
        private INotificationServices _notificationServices;
        private ISubscriptionServices _subscriptionServices;
        private IUserServices _userServices;
        private IUserRoleServices _userRoleServices;
        private IEditionHelper _editionHelper;

        private EditionController _controller;

        [SetUp]
        public void SetUp()
        {
            var unitOfWork = new UnitOfWork();

            // AUTH
            _authUserServices = new UserServices(unitOfWork);
            _roleServices = new RoleServices(unitOfWork);
            _applicationServices = new ApplicationServices(unitOfWork);
            _industryServices = new IndustryServices(unitOfWork);
            _regionServices = new RegionServices(unitOfWork);

            _countryServices = new CountryServices(unitOfWork);
            _editionServices = new EditionServices(unitOfWork);
            _editionCohostServices = new EditionCohostServices(unitOfWork);
            _editionCountryServices = new EditionCountryServices(unitOfWork);
            _editionKeyVisitorServices = new EditionKeyVisitorServices(unitOfWork);
            _editionTranslationServices = new EditionTranslationServices(unitOfWork);
            _editionTranslationSocialMediaServices = new EditionTranslationSocialMediaServices(unitOfWork);
            _editionVisitorServices = new EditionVisitorServices(unitOfWork);
            _eventServices = new EventServices(unitOfWork);
            _eventDirectorServices = new EventDirectorServices(unitOfWork);
            _fileServices = new FileServices(unitOfWork);
            _keyVisitorServices = new KeyVisitorServices(unitOfWork);
            _logServices = new LogServices(unitOfWork);
            _notificationServices = new NotificationServices(unitOfWork);
            _subscriptionServices = new SubscriptionServices(unitOfWork);
            _userServices = new UserServices(unitOfWork);
            _userRoleServices = new UserRoleServices(unitOfWork);

            _editionHelper = new EditionHelper();

            _controller = new EditionController(
                _authUserServices,
                _roleServices,
                _applicationServices,
                _industryServices,
                _regionServices,
                _countryServices,
                _editionServices,
                _editionCohostServices,
                _editionCountryServices,
                _editionKeyVisitorServices,
                _editionTranslationServices,
                _editionTranslationSocialMediaServices,
                _editionVisitorServices,
                _eventServices,
                _eventDirectorServices,
                _fileServices,
                _keyVisitorServices,
                _logServices,
                _notificationServices,
                _subscriptionServices,
                _userServices,
                _userRoleServices,
                _editionHelper
            );
            _controller.SetDefaultUser();

            AutoMapperConfig.Register();
        }

        [Test, Isolated]
        public void Index_ValidRequest_ReturnsEditionIndexModel()
        {
            // Arrange
            var @event = new Event();
            @event.FillWithDefaultValues();
            @event = _context.Events.Add(@event);

            var edition = new Edition();
            edition.FillWithDefaultValues(@event);
            edition = _context.Editions.Add(edition);

            var eventDirector = new EventDirector();
            eventDirector.FillWithDefaultValues(@event);
            _context.EventDirectors.Add(eventDirector);

            _context.SaveChanges();

            // Act
            var result = _controller.Index(edition.EventId);

            // Assert
            Assert.IsInstanceOf<EditionIndexModel>(result.Model);
            Assert.AreEqual(1, ((EditionIndexModel) result.Model).Editions.Count);
        }

        [Test, Isolated]
        public void Details_ValidRequest_ReturnsEditionDetailsModel()
        {
            // Arrange
            var @event = new Event();
            @event.FillWithDefaultValues();
            @event = _context.Events.Add(@event);

            var edition = new Edition();
            edition.FillWithDefaultValues(@event);
            edition = _context.Editions.Add(edition);

            _context.SaveChanges();

            // Act
            var result = _controller.Details(edition.EditionId, null);

            // Assert
            Assert.IsInstanceOf<EditionDetailsModel>(((ViewResult) result).Model);
            Assert.AreEqual(edition.EditionId, ((EditionDetailsModel) ((ViewResult) result).Model).EditionId);
        }

        [Test, Isolated]
        public void Edit_ValidRequest_ReturnsEditionEditModel()
        {
            // Arrange
            var @event = new Event();
            @event.FillWithDefaultValues();
            @event = _context.Events.Add(@event);

            var edition = new Edition();
            edition.FillWithDefaultValues(@event);
            edition = _context.Editions.Add(edition);

            var eventDirector = new EventDirector();
            eventDirector.FillWithDefaultValues(@event);
            _context.EventDirectors.Add(eventDirector);

            _context.SaveChanges();

            // Act
            var result = _controller.Edit(edition.EditionId, null);

            // Assert
            Assert.IsInstanceOf<EditionEditModel>(((ViewResult) result).Model);
            Assert.AreEqual(edition.EditionId, ((EditionEditModel) ((ViewResult) result).Model).EditionId);
        }

        [Test, Isolated]
        public void Delete_ValidRequest_RedirectsToIndexAction()
        {
            // Arrange
            var @event = new Event();
            @event.FillWithDefaultValues();
            @event = _context.Events.Add(@event);

            var edition = new Edition();
            edition.FillWithDefaultValues(@event);
            edition.Status = (byte) EditionStatusType.Draft.GetHashCode();
            edition = _context.Editions.Add(edition);

            var eventDirector = new EventDirector();
            eventDirector.FillWithDefaultValues(@event);
            eventDirector.IsPrimary = true;
            _context.EventDirectors.Add(eventDirector);

            _context.SaveChanges();

            // Act
            var result = _controller.Delete(edition.EditionId);

            // Assert
            Assert.IsInstanceOf<RedirectToRouteResult>(result);
        }

        [Test, Isolated]
        public void _SaveGeneralInfo_ValidRequest_ReturnsJsonResult()
        {
            // Arrange
            var @event = new Event();
            @event.FillWithDefaultValues();
            @event = _context.Events.Add(@event);

            var edition = new Edition();
            edition.FillWithDefaultValues(@event);
            edition = _context.Editions.Add(edition);

            var editionTranslation = new EditionTranslation();
            editionTranslation.FillWithDefaultValues(edition);
            _context.EditionTranslations.Add(editionTranslation);

            var eventDirector = new EventDirector();
            eventDirector.FillWithDefaultValues(@event);
            eventDirector.IsPrimary = true;
            _context.EventDirectors.Add(eventDirector);

            _context.SaveChanges();

            var model = Mapper.Map<Edition, EditionEditGeneralInfoModel>(edition);

            // Act
            var result = _controller._SaveGeneralInfo(model);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            Assert.IsTrue(result.GetValue<bool>("success"));
        }
    }
}
