namespace NavagisInternalTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserDBUniqueUsername : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Users", "Username", unique: true, name: "Username_Index");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "Username_Index");
        }
    }
}
