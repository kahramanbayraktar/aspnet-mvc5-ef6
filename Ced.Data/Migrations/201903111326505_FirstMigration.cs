namespace Ced.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Application",
                c => new
                    {
                        ApplicationId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Code = c.String(nullable: false, maxLength: 10),
                        ClientId = c.String(maxLength: 50),
                        ClientSecret = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RoleId)
                .ForeignKey("dbo.Application", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        UserRoleId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        IndustryId = c.Int(),
                        RegionId = c.Int(),
                        CountryId = c.Int(),
                        Country_CountryCode = c.String(maxLength: 3, unicode: false),
                    })
                .PrimaryKey(t => t.UserRoleId)
                .ForeignKey("dbo.Country", t => t.Country_CountryCode)
                .ForeignKey("dbo.Industry", t => t.IndustryId)
                .ForeignKey("dbo.Region", t => t.RegionId)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId)
                .Index(t => t.IndustryId)
                .Index(t => t.RegionId)
                .Index(t => t.Country_CountryCode);
            
            CreateTable(
                "dbo.Country",
                c => new
                    {
                        CountryCode = c.String(nullable: false, maxLength: 3, unicode: false),
                        CountryId = c.Int(nullable: false),
                        CountryName = c.String(nullable: false, maxLength: 200),
                        CountryCode2 = c.String(nullable: false, maxLength: 2, fixedLength: true, unicode: false),
                        CountryLanguage = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.CountryCode);
            
            CreateTable(
                "dbo.EditionCountry",
                c => new
                    {
                        EditionCountryId = c.Int(nullable: false, identity: true),
                        EditionId = c.Int(nullable: false),
                        CountryCode = c.String(nullable: false, maxLength: 3, unicode: false),
                        RelationType = c.Byte(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EditionCountryId)
                .ForeignKey("dbo.EditionCountryRelationType", t => t.RelationType)
                .ForeignKey("dbo.Country", t => t.CountryCode)
                .Index(t => t.CountryCode)
                .Index(t => t.RelationType);
            
            CreateTable(
                "dbo.EditionCountryRelationType",
                c => new
                    {
                        EditionCountryRelationTypeId = c.Byte(nullable: false),
                        Name = c.String(nullable: false, maxLength: 20, unicode: false),
                    })
                .PrimaryKey(t => t.EditionCountryRelationTypeId);
            
            CreateTable(
                "dbo.Edition",
                c => new
                    {
                        EditionId = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        AxEventId = c.Int(nullable: false),
                        DwEventID = c.Int(nullable: false),
                        EditionName = c.String(maxLength: 300),
                        ReportingName = c.String(maxLength: 255),
                        LocalName = c.String(maxLength: 255),
                        InternationalName = c.String(maxLength: 255),
                        EditionNo = c.Int(nullable: false),
                        Frequency = c.String(maxLength: 60),
                        Country = c.String(maxLength: 255),
                        CountryCode = c.String(maxLength: 3, unicode: false),
                        City = c.String(maxLength: 100),
                        VenueCoordinates = c.String(maxLength: 200),
                        StartDate = c.DateTime(storeType: "date"),
                        EndDate = c.DateTime(storeType: "date"),
                        VisitStartTime = c.Time(nullable: false, precision: 7),
                        VisitEndTime = c.Time(nullable: false, precision: 7),
                        FinancialYearStart = c.Int(),
                        FinancialYearEnd = c.Int(),
                        StartWeekOfYearDiff = c.Int(),
                        StartDayOfYearDiff = c.Int(),
                        DirectorFullName = c.String(maxLength: 60),
                        DirectorEmail = c.String(maxLength: 60),
                        EventTypeCode = c.String(maxLength: 60, unicode: false),
                        Classification = c.String(maxLength: 60),
                        AllDayEvent = c.Boolean(nullable: false),
                        CoHostedEvent = c.Boolean(nullable: false),
                        CoHostedEventCount = c.Int(),
                        TradeShowConnectDisplay = c.Byte(),
                        EventActivity = c.String(maxLength: 60),
                        ManagingOfficeName = c.String(maxLength: 60),
                        ManagingOfficePhone = c.String(maxLength: 50),
                        ManagingOfficeEmail = c.String(maxLength: 80),
                        ManagingOfficeWebsite = c.String(maxLength: 100),
                        DirectorManagingOfficeName = c.String(maxLength: 60),
                        EventWebSite = c.String(maxLength: 100),
                        EventFlagPictureFileName = c.String(maxLength: 500),
                        MarketoPreferenceCenterLink = c.String(maxLength: 255),
                        LocalSqmSold = c.Decimal(precision: 10, scale: 2),
                        InternationalSqmSold = c.Decimal(precision: 10, scale: 2),
                        SqmSold = c.Decimal(precision: 10, scale: 2),
                        LocalExhibitorCount = c.Int(),
                        InternationalExhibitorCount = c.Int(),
                        ExhibitorCount = c.Int(),
                        LocalDelegateCount = c.Int(),
                        InternationalDelegateCount = c.Int(),
                        LocalPaidDelegateCount = c.Int(),
                        InternationalPaidDelegateCount = c.Int(),
                        SponsorCount = c.Byte(),
                        LocalVisitorCount = c.Int(),
                        InternationalVisitorCount = c.Int(),
                        LocalRepeatVisitCount = c.Int(),
                        InternationalRepeatVisitCount = c.Int(),
                        RepeatVisitCount = c.Int(),
                        VisitorCountryCount = c.Int(),
                        NationalGroupCount = c.Short(),
                        ExhibitorCountryCount = c.Short(),
                        OnlineRegistrationCount = c.Int(),
                        OnlineRegisteredVisitorCount = c.Int(),
                        OnlineRegisteredBuyerVisitorCount = c.Int(),
                        LocalExhibitorRetentionRate = c.Decimal(precision: 10, scale: 2),
                        InternationalExhibitorRetentionRate = c.Decimal(precision: 10, scale: 2),
                        ExhibitorRetentionRate = c.Decimal(precision: 10, scale: 2),
                        NPSScoreVisitor = c.Decimal(precision: 10, scale: 2),
                        NPSScoreExhibitor = c.Decimal(precision: 10, scale: 2),
                        NPSSatisfactionVisitor = c.Decimal(precision: 10, scale: 2),
                        NPSSatisfactionExhibitor = c.Decimal(precision: 10, scale: 2),
                        NetEasyScoreVisitor = c.Decimal(precision: 10, scale: 2),
                        NetEasyScoreExhibitor = c.Decimal(precision: 10, scale: 2),
                        PreviousInstanceDwEventId = c.Int(),
                        DisplayOnIteGermany = c.Boolean(),
                        DisplayOnIteAsia = c.Boolean(),
                        DisplayOnIteI = c.Boolean(),
                        DisplayOnItePoland = c.Boolean(),
                        DisplayOnIteModa = c.Boolean(),
                        DisplayOnIteTurkey = c.Boolean(),
                        DisplayOnTradeLink = c.Boolean(),
                        DisplayOnIteUkraine = c.Boolean(),
                        DisplayOnIteBuildInteriors = c.Boolean(),
                        DisplayOnIteFoodDrink = c.Boolean(),
                        DisplayOnIteOilGas = c.Boolean(),
                        DisplayOnIteTravelTourism = c.Boolean(),
                        DisplayOnIteTransportLogistics = c.Boolean(),
                        DisplayOnIteFashion = c.Boolean(),
                        DisplayOnIteSecurity = c.Boolean(),
                        DisplayOnIteBeauty = c.Boolean(),
                        DisplayOnIteHealthCare = c.Boolean(),
                        DisplayOnIteMining = c.Boolean(),
                        DisplayOnIteEngineeringIndustrial = c.Boolean(),
                        HiddenFromWebSites = c.Boolean(),
                        EventOwnershipBEID = c.String(maxLength: 10),
                        EventOwnership = c.String(maxLength: 60),
                        CreateTime = c.DateTime(nullable: false),
                        CreateUser = c.Int(),
                        UpdateTime = c.DateTime(),
                        UpdateUser = c.Int(nullable: false),
                        UpdateTimeByAutoIntegration = c.DateTime(),
                        MatchedKenticoEventId = c.Int(nullable: false),
                        MatchedOn = c.DateTime(),
                        Status = c.Byte(nullable: false),
                        StatusUpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.EditionId)
                .ForeignKey("dbo.Country", t => t.CountryCode)
                .ForeignKey("dbo.Event", t => t.EventId)
                .Index(t => t.EventId)
                .Index(t => t.CountryCode);
            
            CreateTable(
                "dbo.EditionCohost",
                c => new
                    {
                        EditionCohostId = c.Int(nullable: false, identity: true),
                        FirstEditionId = c.Int(nullable: false),
                        SecondEditionId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EditionCohostId)
                .ForeignKey("dbo.Edition", t => t.FirstEditionId)
                .ForeignKey("dbo.Edition", t => t.SecondEditionId)
                .Index(t => t.FirstEditionId)
                .Index(t => t.SecondEditionId);
            
            CreateTable(
                "dbo.EditionKeyVisitor",
                c => new
                    {
                        EditionKeyVisitorId = c.Int(nullable: false, identity: true),
                        EditionId = c.Int(nullable: false),
                        KeyVisitorId = c.Int(nullable: false),
                        Value = c.String(nullable: false, maxLength: 500, unicode: false),
                        EventBEID = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EditionKeyVisitorId)
                .ForeignKey("dbo.KeyVisitor", t => t.KeyVisitorId)
                .ForeignKey("dbo.Edition", t => t.EditionId)
                .Index(t => t.EditionId)
                .Index(t => t.KeyVisitorId);
            
            CreateTable(
                "dbo.KeyVisitor",
                c => new
                    {
                        KeyVisitorId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.KeyVisitorId);
            
            CreateTable(
                "dbo.EditionTranslation",
                c => new
                    {
                        EditionTranslationId = c.Int(nullable: false, identity: true),
                        EditionId = c.Int(nullable: false),
                        LanguageCode = c.String(nullable: false, maxLength: 5),
                        VenueName = c.String(maxLength: 60),
                        MapVenueName = c.String(maxLength: 60),
                        MapVenueFullAddress = c.String(maxLength: 255),
                        Description = c.String(),
                        Summary = c.String(),
                        BookStandUrl = c.String(maxLength: 500),
                        OnlineInvitationUrl = c.String(maxLength: 500),
                        WebLogoFileName = c.String(maxLength: 255),
                        PeopleImageFileName = c.String(maxLength: 255),
                        ProductImageFileName = c.String(maxLength: 255),
                        MasterLogoFileName = c.String(maxLength: 255),
                        CrmLogoFileName = c.String(maxLength: 255),
                        IconFileName = c.String(maxLength: 255),
                        ExhibitorProfile = c.String(maxLength: 500),
                        VisitorProfile = c.String(maxLength: 500),
                        CreateTime = c.DateTime(nullable: false),
                        CreateUser = c.Int(),
                        UpdateTime = c.DateTime(),
                        UpdateUser = c.Int(),
                        UpdateTimeByAutoIntegration = c.DateTime(),
                    })
                .PrimaryKey(t => t.EditionTranslationId)
                .ForeignKey("dbo.Edition", t => t.EditionId)
                .Index(t => t.EditionId);
            
            CreateTable(
                "dbo.EditionTranslationSocialMedia",
                c => new
                    {
                        EditionTranslationSocialMediaId = c.Int(nullable: false, identity: true),
                        EditionTranslationId = c.Int(nullable: false),
                        EditionId = c.Int(nullable: false),
                        SocialMediaId = c.String(nullable: false, maxLength: 20, unicode: false),
                        AccountName = c.String(nullable: false, maxLength: 100),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EditionTranslationSocialMediaId)
                .ForeignKey("dbo.SocialMedia", t => t.SocialMediaId)
                .ForeignKey("dbo.EditionTranslation", t => t.EditionTranslationId)
                .Index(t => t.EditionTranslationId)
                .Index(t => t.SocialMediaId);
            
            CreateTable(
                "dbo.SocialMedia",
                c => new
                    {
                        SocialMediaId = c.String(nullable: false, maxLength: 20, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.SocialMediaId);
            
            CreateTable(
                "dbo.EditionVisitor",
                c => new
                    {
                        EditionVisitorId = c.Int(nullable: false, identity: true),
                        EditionId = c.Int(nullable: false),
                        DayNumber = c.Byte(nullable: false),
                        VisitorCount = c.Short(nullable: false),
                        RepeatVisitCount = c.Short(),
                        OldVisitorCount = c.Short(),
                        NewVisitorCount = c.Short(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EditionVisitorId)
                .ForeignKey("dbo.Edition", t => t.EditionId)
                .Index(t => t.EditionId);
            
            CreateTable(
                "dbo.Event",
                c => new
                    {
                        EventId = c.Int(nullable: false, identity: true),
                        MasterName = c.String(nullable: false, maxLength: 60),
                        MasterCode = c.String(maxLength: 60),
                        Region = c.String(maxLength: 100),
                        EventType = c.String(nullable: false, maxLength: 60),
                        EventTypeCode = c.String(maxLength: 60),
                        Industry = c.String(maxLength: 100),
                        SubIndustry = c.String(maxLength: 100),
                        Brand = c.String(maxLength: 50),
                        EventBusinessClassification = c.String(maxLength: 60),
                        CreateTime = c.DateTime(nullable: false),
                        CreateUser = c.Int(),
                        UpdateTime = c.DateTime(),
                        UpdateUser = c.Int(),
                        UpdateTimeByAutoIntegration = c.DateTime(),
                    })
                .PrimaryKey(t => t.EventId);
            
            CreateTable(
                "dbo.EventDirector",
                c => new
                    {
                        EventDirectorId = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        DirectorEmail = c.String(nullable: false, maxLength: 80),
                        DirectorFullName = c.String(maxLength: 100),
                        ADLogonName = c.String(maxLength: 80),
                        IsPrimary = c.Boolean(),
                        IsAssistant = c.Boolean(),
                        IsAutoGenerated = c.Byte(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EventDirectorId)
                .ForeignKey("dbo.Event", t => t.EventId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Notification",
                c => new
                    {
                        NotificationId = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 100),
                        Description = c.String(maxLength: 500),
                        NotificationType = c.Byte(nullable: false),
                        ReceiverId = c.Int(),
                        ReceiverEmail = c.String(maxLength: 50, unicode: false),
                        Displayed = c.Boolean(),
                        SentByEmail = c.Boolean(),
                        EmailedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        CreatedOn = c.DateTime(nullable: false),
                        DisplayedOn = c.DateTime(),
                        EventId = c.Int(),
                        EditionId = c.Int(),
                    })
                .PrimaryKey(t => t.NotificationId)
                .ForeignKey("dbo.Edition", t => t.EditionId)
                .ForeignKey("dbo.Event", t => t.EventId)
                .Index(t => t.EventId)
                .Index(t => t.EditionId);
            
            CreateTable(
                "dbo.Industry",
                c => new
                    {
                        IndustryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.IndustryId);
            
            CreateTable(
                "dbo.Region",
                c => new
                    {
                        RegionId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.RegionId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 50),
                        UserPrincipalName = c.String(nullable: false, maxLength: 100),
                        Password = c.String(maxLength: 50),
                        Name = c.String(maxLength: 50),
                        Surname = c.String(maxLength: 50),
                        ADLogonName = c.String(maxLength: 50),
                        CreateTime = c.DateTime(),
                        CreateUser = c.Int(),
                        UpdateTime = c.DateTime(),
                        UpdateUser = c.Int(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Clnd_AdUserList",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserFullName = c.String(maxLength: 60),
                        UserLogonName = c.String(maxLength: 100),
                        UserEmail = c.String(maxLength: 100),
                        Pre2000LogonName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Clnd_Custom_Countries",
                c => new
                    {
                        ItemID = c.Int(nullable: false, identity: true),
                        ItemOrder = c.Int(),
                        ItemGUID = c.Guid(nullable: false),
                        CountryName = c.String(nullable: false, maxLength: 500),
                        CountryNameLowerCase = c.String(nullable: false, maxLength: 500),
                        RegionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ItemID);
            
            CreateTable(
                "dbo.Clnd_Custom_IndustrySectors",
                c => new
                    {
                        ItemID = c.Int(nullable: false, identity: true),
                        ItemOrder = c.Int(),
                        ItemGUID = c.Guid(nullable: false),
                        IndustrySectorName = c.String(nullable: false, maxLength: 500),
                        CEDName = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.ItemID);
            
            CreateTable(
                "dbo.Clnd_Custom_Regions",
                c => new
                    {
                        ItemID = c.Int(nullable: false, identity: true),
                        ItemOrder = c.Int(),
                        ItemGUID = c.Guid(nullable: false),
                        RegionName = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.ItemID);
            
            CreateTable(
                "dbo.Clnd_customtable_Organiser",
                c => new
                    {
                        ItemID = c.Int(nullable: false, identity: true),
                        OrganiserName = c.String(nullable: false, maxLength: 500),
                        ItemOrder = c.Int(),
                        ItemGUID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ItemID);
            
            CreateTable(
                "dbo.Clnd_customtable_Venue",
                c => new
                    {
                        ItemID = c.Int(nullable: false, identity: true),
                        ItemOrder = c.Int(),
                        ItemGUID = c.Guid(nullable: false),
                        VenueName = c.String(nullable: false, maxLength: 500),
                        VenueLocation = c.String(nullable: false, maxLength: 500),
                        CEDName = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.ItemID);
            
            CreateTable(
                "dbo.Clnd_EventDirectors",
                c => new
                    {
                        EventDirectorID = c.Int(nullable: false, identity: true),
                        EventID = c.Int(nullable: false),
                        EditionID = c.Int(nullable: false),
                        DwEventID = c.Int(nullable: false),
                        EventBEId = c.String(nullable: false, maxLength: 100),
                        MasterCode = c.String(maxLength: 60),
                        EditionName = c.String(maxLength: 255),
                        MasterName = c.String(nullable: false, maxLength: 60),
                        DirectorFullName = c.String(nullable: false, maxLength: 60),
                        DirectorManagingOfficeName = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.EventDirectorID);
            
            CreateTable(
                "dbo.Clnd_KenticoEvents",
                c => new
                    {
                        KenticoEventID = c.Int(nullable: false),
                        EventBEID = c.Int(),
                        EventName = c.String(maxLength: 150),
                        City = c.String(maxLength: 150),
                        Country = c.String(maxLength: 150),
                        EventDateInt = c.Int(),
                        EventEndDateInt = c.Int(),
                        EventDate = c.DateTime(),
                        EventEndDate = c.DateTime(),
                        EventSummary = c.String(maxLength: 4000),
                        EventDisplayDate = c.String(maxLength: 150),
                        CountryID = c.Int(),
                        IndustrySectorID = c.Int(),
                        IndustrySectorID2 = c.Int(),
                        EventImage = c.String(maxLength: 350),
                        EventBackGroundImage = c.String(maxLength: 350),
                        Telephone = c.String(maxLength: 150),
                        Fax = c.String(maxLength: 150),
                        Website = c.String(maxLength: 150),
                        BookTicketLink = c.String(maxLength: 150),
                        Organiser = c.String(maxLength: 150),
                        EventDetails = c.String(maxLength: 4000),
                        IndustrySector2 = c.String(maxLength: 150),
                        InternationalDial = c.String(maxLength: 150),
                        EmailAddress = c.String(maxLength: 150),
                        EventAllDay = c.Int(),
                        EventLocation = c.String(maxLength: 150),
                        VenueLocation = c.Int(),
                        ITEI = c.Byte(),
                        GiMA = c.Byte(),
                        ASIA = c.Byte(),
                        Turkey = c.Byte(),
                        TradeLink = c.Byte(),
                        MODA = c.Byte(),
                        UpdatedOn = c.DateTime(),
                        Desc = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.KenticoEventID);
            
            CreateTable(
                "dbo.Clnd_Temp_Org",
                c => new
                    {
                        OrginiserIDMst = c.Int(nullable: false, identity: true),
                        ItemID = c.Int(nullable: false),
                        OrganiserName = c.String(nullable: false, maxLength: 500),
                        ItemOrder = c.Int(),
                        ItemGUID = c.Guid(nullable: false),
                        CEDName = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.OrginiserIDMst);
            
            CreateTable(
                "dbo.File",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        FileName = c.String(nullable: false, maxLength: 100),
                        EntityId = c.Int(nullable: false),
                        EntityType = c.String(nullable: false, maxLength: 20, unicode: false),
                        FileType = c.String(nullable: false, maxLength: 20, unicode: false),
                        LanguageCode = c.String(maxLength: 5, fixedLength: true, unicode: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        CreatedByEmail = c.String(nullable: false, maxLength: 50, unicode: false),
                        CreatedByFullName = c.String(nullable: false, maxLength: 100),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                        UpdatedByEmail = c.String(maxLength: 50, unicode: false),
                        UpdatedByFullName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.FileId);
            
            CreateTable(
                "dbo.Log",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        Ip = c.String(nullable: false, maxLength: 50, unicode: false),
                        Url = c.String(nullable: false, maxLength: 500, unicode: false),
                        ActorUserId = c.Int(),
                        ActorUserEmail = c.String(maxLength: 50, unicode: false),
                        Controller = c.String(nullable: false, maxLength: 50, unicode: false),
                        Action = c.String(nullable: false, maxLength: 50, unicode: false),
                        MethodType = c.String(nullable: false, maxLength: 5, unicode: false),
                        EntityType = c.String(maxLength: 50, unicode: false),
                        ActionType = c.String(maxLength: 50, unicode: false),
                        EntityId = c.Int(),
                        EntityName = c.String(maxLength: 255),
                        EventId = c.Int(),
                        EventName = c.String(maxLength: 60),
                        AdditionalInfo = c.String(maxLength: 1000, unicode: false),
                        IsImpersonated = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId);
            
            CreateTable(
                "dbo.Subscription",
                c => new
                    {
                        SubscriptionId = c.Int(nullable: false, identity: true),
                        EditionId = c.Int(nullable: false),
                        UserEmail = c.String(maxLength: 80),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.UserRole", "RegionId", "dbo.Region");
            DropForeignKey("dbo.UserRole", "IndustryId", "dbo.Industry");
            DropForeignKey("dbo.UserRole", "Country_CountryCode", "dbo.Country");
            DropForeignKey("dbo.Notification", "EventId", "dbo.Event");
            DropForeignKey("dbo.Notification", "EditionId", "dbo.Edition");
            DropForeignKey("dbo.EventDirector", "EventId", "dbo.Event");
            DropForeignKey("dbo.Edition", "EventId", "dbo.Event");
            DropForeignKey("dbo.EditionVisitor", "EditionId", "dbo.Edition");
            DropForeignKey("dbo.EditionTranslation", "EditionId", "dbo.Edition");
            DropForeignKey("dbo.EditionTranslationSocialMedia", "EditionTranslationId", "dbo.EditionTranslation");
            DropForeignKey("dbo.EditionTranslationSocialMedia", "SocialMediaId", "dbo.SocialMedia");
            DropForeignKey("dbo.EditionKeyVisitor", "EditionId", "dbo.Edition");
            DropForeignKey("dbo.EditionKeyVisitor", "KeyVisitorId", "dbo.KeyVisitor");
            DropForeignKey("dbo.EditionCohost", "SecondEditionId", "dbo.Edition");
            DropForeignKey("dbo.EditionCohost", "FirstEditionId", "dbo.Edition");
            DropForeignKey("dbo.Edition", "CountryCode", "dbo.Country");
            DropForeignKey("dbo.EditionCountry", "CountryCode", "dbo.Country");
            DropForeignKey("dbo.EditionCountry", "RelationType", "dbo.EditionCountryRelationType");
            DropForeignKey("dbo.Role", "ApplicationId", "dbo.Application");
            DropIndex("dbo.Notification", new[] { "EditionId" });
            DropIndex("dbo.Notification", new[] { "EventId" });
            DropIndex("dbo.EventDirector", new[] { "EventId" });
            DropIndex("dbo.EditionVisitor", new[] { "EditionId" });
            DropIndex("dbo.EditionTranslationSocialMedia", new[] { "SocialMediaId" });
            DropIndex("dbo.EditionTranslationSocialMedia", new[] { "EditionTranslationId" });
            DropIndex("dbo.EditionTranslation", new[] { "EditionId" });
            DropIndex("dbo.EditionKeyVisitor", new[] { "KeyVisitorId" });
            DropIndex("dbo.EditionKeyVisitor", new[] { "EditionId" });
            DropIndex("dbo.EditionCohost", new[] { "SecondEditionId" });
            DropIndex("dbo.EditionCohost", new[] { "FirstEditionId" });
            DropIndex("dbo.Edition", new[] { "CountryCode" });
            DropIndex("dbo.Edition", new[] { "EventId" });
            DropIndex("dbo.EditionCountry", new[] { "RelationType" });
            DropIndex("dbo.EditionCountry", new[] { "CountryCode" });
            DropIndex("dbo.UserRole", new[] { "Country_CountryCode" });
            DropIndex("dbo.UserRole", new[] { "RegionId" });
            DropIndex("dbo.UserRole", new[] { "IndustryId" });
            DropIndex("dbo.UserRole", new[] { "RoleId" });
            DropIndex("dbo.UserRole", new[] { "UserId" });
            DropIndex("dbo.Role", new[] { "ApplicationId" });
            DropTable("dbo.Subscription");
            DropTable("dbo.Log");
            DropTable("dbo.File");
            DropTable("dbo.Clnd_Temp_Org");
            DropTable("dbo.Clnd_KenticoEvents");
            DropTable("dbo.Clnd_EventDirectors");
            DropTable("dbo.Clnd_customtable_Venue");
            DropTable("dbo.Clnd_customtable_Organiser");
            DropTable("dbo.Clnd_Custom_Regions");
            DropTable("dbo.Clnd_Custom_IndustrySectors");
            DropTable("dbo.Clnd_Custom_Countries");
            DropTable("dbo.Clnd_AdUserList");
            DropTable("dbo.User");
            DropTable("dbo.Region");
            DropTable("dbo.Industry");
            DropTable("dbo.Notification");
            DropTable("dbo.EventDirector");
            DropTable("dbo.Event");
            DropTable("dbo.EditionVisitor");
            DropTable("dbo.SocialMedia");
            DropTable("dbo.EditionTranslationSocialMedia");
            DropTable("dbo.EditionTranslation");
            DropTable("dbo.KeyVisitor");
            DropTable("dbo.EditionKeyVisitor");
            DropTable("dbo.EditionCohost");
            DropTable("dbo.Edition");
            DropTable("dbo.EditionCountryRelationType");
            DropTable("dbo.EditionCountry");
            DropTable("dbo.Country");
            DropTable("dbo.UserRole");
            DropTable("dbo.Role");
            DropTable("dbo.Application");
        }
    }
}
