using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShireBank.Shared.Migrations
{
    public partial class Migration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Balance",
                table: "Accounts",
                type: "REAL",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldDefaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Balance",
                table: "Accounts",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(float),
                oldType: "REAL",
                oldDefaultValue: 0f);
        }
    }
}
