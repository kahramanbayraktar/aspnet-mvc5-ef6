namespace Ced.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedImageFieldsAndPromotedFieldToEdition2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EditionTranslation", "PromotedLogoFileName", c => c.String(maxLength: 255));
            AddColumn("dbo.EditionTranslation", "DetailsImageFileName", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EditionTranslation", "DetailsImageFileName");
            DropColumn("dbo.EditionTranslation", "PromotedLogoFileName");
        }
    }
}
