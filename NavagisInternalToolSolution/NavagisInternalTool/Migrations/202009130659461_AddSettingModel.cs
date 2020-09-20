namespace NavagisInternalTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSettingModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.String(nullable: false, maxLength: 255),
                        ClientSecret = c.String(nullable: false, maxLength: 255),
                        ServiceAccountFilePath = c.String(nullable: false, maxLength: 255),
                        ServiceAccountEmail = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Settings");
        }
    }
}
