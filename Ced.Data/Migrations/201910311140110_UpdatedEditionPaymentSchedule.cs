namespace Ced.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedEditionPaymentSchedule : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EditionPaymentSchedule", "Installment2Percentage", c => c.Int());
            AlterColumn("dbo.EditionPaymentSchedule", "Installment2DueDate", c => c.DateTime(storeType: "date"));
            AlterColumn("dbo.EditionPaymentSchedule", "Installment3Percentage", c => c.Int());
            AlterColumn("dbo.EditionPaymentSchedule", "Installment3DueDate", c => c.DateTime(storeType: "date"));
            AlterColumn("dbo.EditionPaymentSchedule", "Installment4Percentage", c => c.Int());
            AlterColumn("dbo.EditionPaymentSchedule", "Installment4DueDate", c => c.DateTime(storeType: "date"));
            AlterColumn("dbo.EditionPaymentSchedule", "Installment5Percentage", c => c.Int());
            AlterColumn("dbo.EditionPaymentSchedule", "Installment5DueDate", c => c.DateTime(storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EditionPaymentSchedule", "Installment5DueDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.EditionPaymentSchedule", "Installment5Percentage", c => c.Int(nullable: false));
            AlterColumn("dbo.EditionPaymentSchedule", "Installment4DueDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.EditionPaymentSchedule", "Installment4Percentage", c => c.Int(nullable: false));
            AlterColumn("dbo.EditionPaymentSchedule", "Installment3DueDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.EditionPaymentSchedule", "Installment3Percentage", c => c.Int(nullable: false));
            AlterColumn("dbo.EditionPaymentSchedule", "Installment2DueDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.EditionPaymentSchedule", "Installment2Percentage", c => c.Int(nullable: false));
        }
    }
}
