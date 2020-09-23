namespace NavagisProjRegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Users", newName: "AdminUsers");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.AdminUsers", newName: "Users");
        }
    }
}
