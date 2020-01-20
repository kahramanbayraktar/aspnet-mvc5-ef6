namespace Ced.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedEditionPaymentScheduleScheme : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EditionPaymentSchedule", "ActivationDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.EditionPaymentSchedule", "ExpiryDate", c => c.DateTime(nullable: false, storeType: "date"));
            DropColumn("dbo.EditionPaymentSchedule", "StartDate");
            DropColumn("dbo.EditionPaymentSchedule", "EndDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EditionPaymentSchedule", "EndDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.EditionPaymentSchedule", "StartDate", c => c.DateTime(nullable: false, storeType: "date"));
            DropColumn("dbo.EditionPaymentSchedule", "ExpiryDate");
            DropColumn("dbo.EditionPaymentSchedule", "ActivationDate");
        }
    }
}
