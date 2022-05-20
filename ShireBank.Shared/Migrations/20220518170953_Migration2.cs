using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShireBank.Shared.Migrations
{
    public partial class Migration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Accounts_FirstName_LastName",
                table: "Accounts",
                columns: new[] { "FirstName", "LastName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Accounts_FirstName_LastName",
                table: "Accounts");
        }
    }
}
