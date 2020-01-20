using Ced.Data.GenericRepository;
using Ced.Data.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace Ced.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly CedContext _context;
        private GenericRepository<Country> _countryRepository;
        private GenericRepository<Edition> _editionRepository;
        private GenericRepository<EditionCohost> _editionCohostRepository;
        private GenericRepository<EditionCountry> _editionCountryRepository;
        private GenericRepository<EditionDiscountApprover> _editionDiscountApproverRepository;
        private GenericRepository<EditionKeyVisitor> _editionKeyVisitorRepository;
        private GenericRepository<EditionPaymentSchedule> _editionPaymentScheduleRepository;
        private GenericRepository<EditionSection> _editionSectionRepository;
        private GenericRepository<EditionTranslation> _editionTranslationRepository;
        private GenericRepository<EditionTranslationSocialMedia> _editionTranslationSocialMediaRepository;
        private GenericRepository<EditionVisitor> _editionVisitorRepository;
        private GenericRepository<Event> _eventRepository;
        private GenericRepository<EventDirector> _eventDirectorRepository;
        private GenericRepository<File> _fileRepository;
        private GenericRepository<KeyVisitor> _keyVisitorRepository;
        private GenericRepository<Log> _logRepository;
        private GenericRepository<Notification> _notificationRepository;
        private GenericRepository<SocialMedia> _socialMediaRepository;
        private GenericRepository<Subscription> _subscriptionRepository;

        // AUTH
        private GenericRepository<Application> _applicationRepository;
        //private GenericRepository<Country> _countryRepository;
        private GenericRepository<Industry> _industryRepository;
        private GenericRepository<Region> _regionRepository;
        private GenericRepository<Role> _roleRepository;
        private GenericRepository<User> _userRepository;
        private GenericRepository<UserRole> _userRoleRepository;

        public UnitOfWork()
        {
            _context = new CedContext();
        }

        #region Public repository creation properties

        public GenericRepository<Country> CountryRepository
            => _countryRepository ?? (_countryRepository = new GenericRepository<Country>(_context));

        public GenericRepository<Edition> EditionRepository
            => _editionRepository ?? (_editionRepository = new GenericRepository<Edition>(_context));

        public GenericRepository<EditionCohost> EditionCohostRepository
            => _editionCohostRepository ?? (_editionCohostRepository = new GenericRepository<EditionCohost>(_context));

        public GenericRepository<EditionCountry> EditionCountryRepository
            => _editionCountryRepository ?? (_editionCountryRepository = new GenericRepository<EditionCountry>(_context));

        public GenericRepository<EditionDiscountApprover> EditionDiscountApproverRepository
            => _editionDiscountApproverRepository ?? (_editionDiscountApproverRepository = new GenericRepository<EditionDiscountApprover>(_context));

        public GenericRepository<EditionKeyVisitor> EditionKeyVisitorRepository
            => _editionKeyVisitorRepository ?? (_editionKeyVisitorRepository = new GenericRepository<EditionKeyVisitor>(_context));

        public GenericRepository<EditionSection> EditionSectionRepository
            => _editionSectionRepository ?? (_editionSectionRepository = new GenericRepository<EditionSection>(_context));

        public GenericRepository<EditionPaymentSchedule> EditionPaymentScheduleRepository
            => _editionPaymentScheduleRepository ?? (_editionPaymentScheduleRepository = new GenericRepository<EditionPaymentSchedule>(_context));

        public GenericRepository<EditionTranslation> EditionTranslationRepository
            => _editionTranslationRepository ?? (_editionTranslationRepository = new GenericRepository<EditionTranslation>(_context));

        public GenericRepository<EditionTranslationSocialMedia> EditionTranslationSocialMediaRepository
            => _editionTranslationSocialMediaRepository ??
                (_editionTranslationSocialMediaRepository = new GenericRepository<EditionTranslationSocialMedia>(_context));

        public GenericRepository<EditionVisitor> EditionVisitorRepository
            => _editionVisitorRepository ??
               (_editionVisitorRepository = new GenericRepository<EditionVisitor>(_context));

        public GenericRepository<Event> EventRepository => _eventRepository ?? (_eventRepository = new GenericRepository<Event>(_context));

        public GenericRepository<EventDirector> EventDirectorRepository
            => _eventDirectorRepository ?? (_eventDirectorRepository = new GenericRepository<EventDirector>(_context));

        public GenericRepository<File> FileRepository => _fileRepository ?? (_fileRepository = new GenericRepository<File>(_context));

        public GenericRepository<KeyVisitor> KeyVisitorRepository
            => _keyVisitorRepository ?? (_keyVisitorRepository = new GenericRepository<KeyVisitor>(_context));

        public GenericRepository<Log> LogRepository => _logRepository ?? (_logRepository = new GenericRepository<Log>(_context));

        public GenericRepository<Notification> NotificationRepository
            => _notificationRepository ?? (_notificationRepository = new GenericRepository<Notification>(_context));

        public GenericRepository<SocialMedia> SocialMediaRepository
            => _socialMediaRepository ?? (_socialMediaRepository = new GenericRepository<SocialMedia>(_context));

        public GenericRepository<Subscription> SubscriptionRepository
            => _subscriptionRepository ?? (_subscriptionRepository = new GenericRepository<Subscription>(_context));

        // AUTH
        public GenericRepository<Application> ApplicationRepository
        {
            get
            {
                if (this._applicationRepository == null)
                    this._applicationRepository = new GenericRepository<Application>(_context);
                return _applicationRepository;
            }
        }

        //public GenericRepository<Country> CountryRepository
        //{
        //    get
        //    {
        //        if (this._countryRepository == null)
        //            this._countryRepository = new GenericRepository<Country>(_context);
        //        return _countryRepository;
        //    }
        //}

        public GenericRepository<Industry> IndustryRepository
        {
            get
            {
                if (this._industryRepository == null)
                    this._industryRepository = new GenericRepository<Industry>(_context);
                return _industryRepository;
            }
        }

        public GenericRepository<Region> RegionRepository
        {
            get
            {
                if (this._regionRepository == null)
                    this._regionRepository = new GenericRepository<Region>(_context);
                return _regionRepository;
            }
        }

        public GenericRepository<Role> RoleRepository
        {
            get
            {
                if (this._roleRepository == null)
                    this._roleRepository = new GenericRepository<Role>(_context);
                return _roleRepository;
            }
        }

        public GenericRepository<User> UserRepository
        {
            get
            {
                if (this._userRepository == null)
                    this._userRepository = new GenericRepository<User>(_context);
                return _userRepository;
            }
        }

        public GenericRepository<UserRole> UserRoleRepository
        {
            get
            {
                if (this._userRoleRepository == null)
                    this._userRoleRepository = new GenericRepository<UserRole>(_context);
                return _userRoleRepository;
            }
        }

        #endregion

        #region Public methods

        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    outputLines.Add($"{DateTime.Now}: Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors:");
                    outputLines.AddRange(eve.ValidationErrors.Select(ve => $"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\""));
                }
                System.IO.File.AppendAllLines(ConfigurationManager.AppSettings["ErrorLogFilePath"], outputLines);

                throw new Exception(string.Join(" | ", outputLines));
            }
        }

        public DbRawSqlQuery<T> SqlQuery<T>(string sql, params object[] parameters)
        {
            return _context.Database.SqlQuery<T>(sql, parameters);
        }

        #endregion

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Debug.WriteLine("UnitOfWork is being disposed");
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
