namespace NavagisInternalTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateSetting : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Settings(ClientId,ClientSecret,ServiceAccountFilePath,ServiceAccountEmail) VALUES('Enter ClientId','Enter Client Secret','Enter Service Acount File Path','Enter Service Account Email')");
        }
        
        public override void Down()
        {
        }
    }
}



