namespace Ced.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRussiaAndEurasiaRegionsToEdition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Edition", "DisplayOnIteRussia", c => c.Boolean());
            AddColumn("dbo.Edition", "DisplayOnIteEurasia", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Edition", "DisplayOnIteEurasia");
            DropColumn("dbo.Edition", "DisplayOnIteRussia");
        }
    }
}
