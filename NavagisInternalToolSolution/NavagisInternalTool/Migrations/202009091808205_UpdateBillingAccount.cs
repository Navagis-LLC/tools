namespace NavagisInternalTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateBillingAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillingAccounts", "BillingAccountName", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.BillingAccounts", "Description", c => c.String(maxLength: 255));
            DropColumn("dbo.BillingAccounts", "ClientId");
            DropColumn("dbo.BillingAccounts", "ClientSecret");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BillingAccounts", "ClientSecret", c => c.String(maxLength: 255));
            AddColumn("dbo.BillingAccounts", "ClientId", c => c.String(nullable: false, maxLength: 255));
            DropColumn("dbo.BillingAccounts", "Description");
            DropColumn("dbo.BillingAccounts", "BillingAccountName");
        }
    }
}
