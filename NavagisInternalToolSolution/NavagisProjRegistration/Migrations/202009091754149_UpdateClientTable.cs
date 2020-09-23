namespace NavagisProjRegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateClientTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Email", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.Clients", "FirstName", c => c.String(maxLength: 255));
            AddColumn("dbo.Clients", "LastName", c => c.String(maxLength: 255));
            DropColumn("dbo.Clients", "ClientId");
            DropColumn("dbo.Clients", "ClientSecret");
            DropColumn("dbo.Clients", "BillingAccountName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "BillingAccountName", c => c.String(maxLength: 255));
            AddColumn("dbo.Clients", "ClientSecret", c => c.String(maxLength: 255));
            AddColumn("dbo.Clients", "ClientId", c => c.String(nullable: false, maxLength: 255));
            DropColumn("dbo.Clients", "LastName");
            DropColumn("dbo.Clients", "FirstName");
            DropColumn("dbo.Clients", "Email");
        }
    }
}
