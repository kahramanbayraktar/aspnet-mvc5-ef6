namespace Ced.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedImageFieldsAndPromotedFieldToEdition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Edition", "Promoted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Edition", "Promoted");
        }
    }
}
