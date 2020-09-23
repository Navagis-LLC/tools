namespace NavagisProjRegistration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdminResetPassword : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminResetPasswords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResetCode = c.String(nullable: false, maxLength: 10),
                        AdminUserId = c.Int(nullable: false),
                        ReportDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ResetCode, unique: true, name: "ResetCode_Index");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.AdminResetPasswords", "ResetCode_Index");
            DropTable("dbo.AdminResetPasswords");
        }
    }
}
