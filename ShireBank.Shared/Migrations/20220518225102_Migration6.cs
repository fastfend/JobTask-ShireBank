using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShireBank.Shared.Migrations
{
    public partial class Migration6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "Timestamp",
                table: "Accounts",
                type: "BLOB",
                rowVersion: true,
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Accounts");

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Accounts",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValue: 0L);
        }
    }
}
