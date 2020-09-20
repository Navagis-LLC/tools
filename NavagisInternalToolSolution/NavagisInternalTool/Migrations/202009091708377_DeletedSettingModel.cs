namespace NavagisInternalTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeletedSettingModel : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Settings");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.String(nullable: false, maxLength: 255),
                        ClientSecret = c.String(nullable: false, maxLength: 255),
                        BillingAccountName = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
