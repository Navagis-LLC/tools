namespace NavagisProjRegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSettingTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Settings", "ServiceAccountFilePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Settings", "ServiceAccountFilePath", c => c.String(nullable: false, maxLength: 255));
        }
    }
}
