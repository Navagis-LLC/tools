namespace NavagisProjRegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBillingAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillingAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.String(nullable: false, maxLength: 255),
                        ClientSecret = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BillingAccounts");
        }
    }
}
