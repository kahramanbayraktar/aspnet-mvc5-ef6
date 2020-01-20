using System.Data.Entity;

namespace Ced.Data.Models
{
    public partial class CedContext : DbContext
    {
        public CedContext()
            : base("name=CedContext")
        {
        }

        public virtual DbSet<Clnd_AdUserList> Clnd_AdUserList { get; set; }
        public virtual DbSet<Clnd_Custom_Countries> Clnd_Custom_Countries { get; set; }
        public virtual DbSet<Clnd_Custom_IndustrySectors> Clnd_Custom_IndustrySectors { get; set; }
        public virtual DbSet<Clnd_Custom_Regions> Clnd_Custom_Regions { get; set; }
        public virtual DbSet<Clnd_customtable_Organiser> Clnd_customtable_Organiser { get; set; }
        public virtual DbSet<Clnd_customtable_Venue> Clnd_customtable_Venue { get; set; }
        public virtual DbSet<Clnd_EventDirectors> Clnd_EventDirectors { get; set; }
        public virtual DbSet<Clnd_KenticoEvents> Clnd_KenticoEvents { get; set; }
        public virtual DbSet<Clnd_Temp_Org> Clnd_Temp_Org { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Edition> Editions { get; set; }
        public virtual DbSet<EditionCohost> EditionCohosts { get; set; }
        public virtual DbSet<EditionCountry> EditionCountries { get; set; }
        public virtual DbSet<EditionCountryRelationType> EditionCountryRelationTypes { get; set; }
        public virtual DbSet<EditionDiscountApprover> EditionDiscountApprovers { get; set; }
        public virtual DbSet<EditionKeyVisitor> EditionKeyVisitors { get; set; }
        public virtual DbSet<EditionPaymentSchedule> EditionPaymentSchedules { get; set; }
        public virtual DbSet<EditionSection> EditionSections { get; set; }
        public virtual DbSet<EditionTranslation> EditionTranslations { get; set; }
        public virtual DbSet<EditionTranslationSocialMedia> EditionTranslationSocialMedias { get; set; }
        public virtual DbSet<EditionVisitor> EditionVisitors { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventDirector> EventDirectors { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<KeyVisitor> KeyVisitors { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<SocialMedia> SocialMedias { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        //public virtual DbSet<Vw_EventInformations> Vw_EventInformations { get; set; }

        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Industry> Industries { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>()
                .Property(e => e.CountryCode)
                .IsUnicode(false);

            modelBuilder.Entity<Country>()
                .Property(e => e.CountryCode2)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Country>()
                .HasMany(e => e.EditionCountries)
                .WithRequired(e => e.Country)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Edition>()
                .Property(e => e.CountryCode)
                .IsUnicode(false);

            modelBuilder.Entity<Edition>()
                .Property(e => e.EventTypeCode)
                .IsUnicode(false);

            modelBuilder.Entity<Edition>()
                .Property(e => e.LocalSqmSold)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .Property(e => e.InternationalSqmSold)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .Property(e => e.SqmSold)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .Property(e => e.LocalExhibitorRetentionRate)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .Property(e => e.InternationalExhibitorRetentionRate)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .Property(e => e.ExhibitorRetentionRate)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .Property(e => e.NPSScoreVisitor)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .Property(e => e.NPSScoreExhibitor)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .Property(e => e.NPSSatisfactionVisitor)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .Property(e => e.NPSSatisfactionExhibitor)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .Property(e => e.NetEasyScoreVisitor)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .Property(e => e.NetEasyScoreExhibitor)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Edition>()
                .HasMany(e => e.EditionCohosts)
                .WithRequired(e => e.Edition)
                .HasForeignKey(e => e.FirstEditionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Edition>()
                .HasMany(e => e.EditionCohosts1)
                .WithRequired(e => e.Edition1)
                .HasForeignKey(e => e.SecondEditionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Edition>()
                .HasMany(e => e.EditionKeyVisitors)
                .WithRequired(e => e.Edition)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Edition>()
                .HasMany(e => e.EditionTranslations)
                .WithRequired(e => e.Edition)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Edition>()
                .HasMany(e => e.EditionVisitors)
                .WithRequired(e => e.Edition)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EditionCountry>()
                .Property(e => e.CountryCode)
                .IsUnicode(false);

            modelBuilder.Entity<EditionCountryRelationType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<EditionCountryRelationType>()
                .HasMany(e => e.EditionCountries)
                .WithRequired(e => e.EditionCountryRelationType)
                .HasForeignKey(e => e.RelationType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EditionKeyVisitor>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<EditionTranslation>()
                .HasMany(e => e.EditionTranslationSocialMedias)
                .WithRequired(e => e.EditionTranslation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EditionTranslationSocialMedia>()
                .Property(e => e.SocialMediaId)
                .IsUnicode(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Editions)
                .WithRequired(e => e.Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventDirectors)
                .WithRequired(e => e.Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<File>()
                .Property(e => e.EntityType)
                .IsUnicode(false);

            modelBuilder.Entity<File>()
                .Property(e => e.FileType)
                .IsUnicode(false);

            modelBuilder.Entity<File>()
                .Property(e => e.LanguageCode)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<File>()
                .Property(e => e.CreatedByEmail)
                .IsUnicode(false);

            modelBuilder.Entity<File>()
                .Property(e => e.UpdatedByEmail)
                .IsUnicode(false);

            modelBuilder.Entity<KeyVisitor>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<KeyVisitor>()
                .HasMany(e => e.EditionKeyVisitors)
                .WithRequired(e => e.KeyVisitor)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Ip)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Url)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.ActorUserEmail)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Controller)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Action)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.MethodType)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.EntityType)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.ActionType)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.AdditionalInfo)
                .IsUnicode(false);

            modelBuilder.Entity<Notification>()
                .Property(e => e.ReceiverEmail)
                .IsUnicode(false);

            modelBuilder.Entity<SocialMedia>()
                .Property(e => e.SocialMediaId)
                .IsUnicode(false);

            modelBuilder.Entity<SocialMedia>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<SocialMedia>()
                .HasMany(e => e.EditionTranslationSocialMedias)
                .WithRequired(e => e.SocialMedia)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Vw_EventInformations>()
            //    .Property(e => e.EventDisplayDate)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Vw_EventInformations>()
            //    .Property(e => e.InternationalDial)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Vw_EventInformations>()
            //    .Property(e => e.DocumentCulture)
            //    .IsUnicode(false);

            //modelBuilder.Entity<Vw_EventInformations>()
            //    .Property(e => e.CountryISO)
            //    .IsUnicode(false);
        }
    }
}
