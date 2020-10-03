using Microsoft.EntityFrameworkCore.Migrations;

namespace RegisterProjectSpice.Migrations
{
    public partial class SeedAdminTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.Sql("INSERT INTO AdminUsers(Username,Password,IsAdmin) VALUES('admin@navagis.com','test',1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
