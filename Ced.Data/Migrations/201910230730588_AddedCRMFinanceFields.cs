namespace Ced.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCRMFinanceFields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EditionDiscountApprover",
                c => new
                    {
                        EditionDiscountApproverId = c.Int(nullable: false, identity: true),
                        EditionId = c.Int(nullable: false),
                        ApprovingUser = c.String(nullable: false, maxLength: 100),
                        ApprovalLowerPercentage = c.Int(nullable: false),
                        ApprovalUpperPercentage = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EditionDiscountApproverId)
                .ForeignKey("dbo.Edition", t => t.EditionId, cascadeDelete: true)
                .Index(t => t.EditionId);
            
            CreateTable(
                "dbo.EditionPaymentSchedule",
                c => new
                    {
                        EditionPaymentScheduleId = c.Int(nullable: false, identity: true),
                        EditionId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        StartDate = c.DateTime(nullable: false, storeType: "date"),
                        EndDate = c.DateTime(nullable: false, storeType: "date"),
                        Installment1Percentage = c.Int(nullable: false),
                        Installment1DueDate = c.DateTime(nullable: false, storeType: "date"),
                        Installment2Percentage = c.Int(nullable: false),
                        Installment2DueDate = c.DateTime(nullable: false, storeType: "date"),
                        Installment3Percentage = c.Int(nullable: false),
                        Installment3DueDate = c.DateTime(nullable: false, storeType: "date"),
                        Installment4Percentage = c.Int(nullable: false),
                        Installment4DueDate = c.DateTime(nullable: false, storeType: "date"),
                        Installment5Percentage = c.Int(nullable: false),
                        Installment5DueDate = c.DateTime(nullable: false, storeType: "date"),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EditionPaymentScheduleId)
                .ForeignKey("dbo.Edition", t => t.EditionId, cascadeDelete: true)
                .Index(t => t.EditionId);
            
            CreateTable(
                "dbo.EditionSection",
                c => new
                    {
                        EditionSectionId = c.Int(nullable: false, identity: true),
                        EditionId = c.Int(nullable: false),
                        Sections = c.String(nullable: false, maxLength: 500),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EditionSectionId)
                .ForeignKey("dbo.Edition", t => t.EditionId, cascadeDelete: true)
                .Index(t => t.EditionId);
            
            AddColumn("dbo.Edition", "CoolOffPeriodStartDate", c => c.DateTime(storeType: "date"));
            AddColumn("dbo.Edition", "CoolOffPeriodEndDate", c => c.DateTime(storeType: "date"));
            AddColumn("dbo.Edition", "ExchangeRateTable", c => c.String(maxLength: 10));
            AddColumn("dbo.EditionTranslation", "InternationalDate", c => c.String(maxLength: 100));
            AddColumn("dbo.EditionTranslation", "LocalDate", c => c.String(maxLength: 100));
            AddColumn("dbo.EditionTranslation", "CountryLocalName", c => c.String(maxLength: 100));
            AddColumn("dbo.EditionTranslation", "CityLocalName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EditionSection", "EditionId", "dbo.Edition");
            DropForeignKey("dbo.EditionPaymentSchedule", "EditionId", "dbo.Edition");
            DropForeignKey("dbo.EditionDiscountApprover", "EditionId", "dbo.Edition");
            DropIndex("dbo.EditionSection", new[] { "EditionId" });
            DropIndex("dbo.EditionPaymentSchedule", new[] { "EditionId" });
            DropIndex("dbo.EditionDiscountApprover", new[] { "EditionId" });
            DropColumn("dbo.EditionTranslation", "CityLocalName");
            DropColumn("dbo.EditionTranslation", "CountryLocalName");
            DropColumn("dbo.EditionTranslation", "LocalDate");
            DropColumn("dbo.EditionTranslation", "InternationalDate");
            DropColumn("dbo.Edition", "ExchangeRateTable");
            DropColumn("dbo.Edition", "CoolOffPeriodEndDate");
            DropColumn("dbo.Edition", "CoolOffPeriodStartDate");
            DropTable("dbo.EditionSection");
            DropTable("dbo.EditionPaymentSchedule");
            DropTable("dbo.EditionDiscountApprover");
        }
    }
}
