namespace Ced.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedSomeFieldsFromEditionTranslationToEdition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Edition", "CountryLocalName", c => c.String(maxLength: 100));
            AddColumn("dbo.Edition", "CityLocalName", c => c.String(maxLength: 100));
            AddColumn("dbo.Edition", "InternationalDate", c => c.String(maxLength: 100));
            AddColumn("dbo.Edition", "LocalDate", c => c.String(maxLength: 100));
            DropColumn("dbo.EditionTranslation", "InternationalDate");
            DropColumn("dbo.EditionTranslation", "LocalDate");
            DropColumn("dbo.EditionTranslation", "CountryLocalName");
            DropColumn("dbo.EditionTranslation", "CityLocalName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EditionTranslation", "CityLocalName", c => c.String(maxLength: 100));
            AddColumn("dbo.EditionTranslation", "CountryLocalName", c => c.String(maxLength: 100));
            AddColumn("dbo.EditionTranslation", "LocalDate", c => c.String(maxLength: 100));
            AddColumn("dbo.EditionTranslation", "InternationalDate", c => c.String(maxLength: 100));
            DropColumn("dbo.Edition", "LocalDate");
            DropColumn("dbo.Edition", "InternationalDate");
            DropColumn("dbo.Edition", "CityLocalName");
            DropColumn("dbo.Edition", "CountryLocalName");
        }
    }
}
