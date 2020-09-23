namespace NavagisProjRegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class applyAnnotationToSetting : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Settings", "ClientId", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Settings", "ClientSecret", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Settings", "BillingAccountName", c => c.String(nullable: false, maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Settings", "BillingAccountName", c => c.String());
            AlterColumn("dbo.Settings", "ClientSecret", c => c.String());
            AlterColumn("dbo.Settings", "ClientId", c => c.String());
        }
    }
}
