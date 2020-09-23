namespace NavagisProjRegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSeedAdminuser : DbMigration
    {
        public override void Up()
        {
            Sql("TRUNCATE TABLE AdminUsers");
            Sql("INSERT INTO AdminUsers(Username,Password,IsAdmin) VALUES('admin@navagis.com','test',1)");
        }
        
        public override void Down()
        {
        }
    }
}
