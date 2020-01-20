namespace Ced.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedExchangeRateTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Edition", "ExchangeRateTable");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Edition", "ExchangeRateTable", c => c.String(maxLength: 10));
        }
    }
}
