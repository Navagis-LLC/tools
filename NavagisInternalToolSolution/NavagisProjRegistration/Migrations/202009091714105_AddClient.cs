namespace NavagisProjRegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClient : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.String(nullable: false, maxLength: 255),
                        ClientSecret = c.String(maxLength: 255),
                        BillingAccountName = c.String(maxLength: 255),
                        BillingAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingAccounts", t => t.BillingAccountId, cascadeDelete: true)
                .Index(t => t.BillingAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Clients", "BillingAccountId", "dbo.BillingAccounts");
            DropIndex("dbo.Clients", new[] { "BillingAccountId" });
            DropTable("dbo.Clients");
        }
    }
}
