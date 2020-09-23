namespace NavagisProjRegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatAdminUserAddNames : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AdminUsers", "Username_Index");
            AddColumn("dbo.AdminUsers", "FirstName", c => c.String(maxLength: 255));
            AddColumn("dbo.AdminUsers", "LastName", c => c.String(maxLength: 255));
            AlterColumn("dbo.AdminUsers", "Username", c => c.String(nullable: false, maxLength: 70));
            CreateIndex("dbo.AdminUsers", "Username", unique: true, name: "Username_Index");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AdminUsers", "Username_Index");
            AlterColumn("dbo.AdminUsers", "Username", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.AdminUsers", "LastName");
            DropColumn("dbo.AdminUsers", "FirstName");
            CreateIndex("dbo.AdminUsers", "Username", unique: true, name: "Username_Index");
        }
    }
}
