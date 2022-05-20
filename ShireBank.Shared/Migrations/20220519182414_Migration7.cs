using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShireBank.Shared.Migrations
{
    public partial class Migration7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Transactions");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Transactions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 19, 20, 24, 14, 753, DateTimeKind.Local).AddTicks(1160));

            migrationBuilder.AddColumn<string>(
                name: "Timestamp",
                table: "Transactions",
                type: "BLOB",
                rowVersion: true,
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Transactions");

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Transactions",
                type: "BLOB",
                rowVersion: true,
                nullable: true);
        }
    }
}
