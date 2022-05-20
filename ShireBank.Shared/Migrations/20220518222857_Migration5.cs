using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShireBank.Shared.Migrations
{
    public partial class Migration5 : Migration
    {
        private static string _triggerQuery = @"CREATE TRIGGER Set{0}RowVersion{1}
            AFTER {1} ON {0}
            BEGIN
                UPDATE {0}
                SET RowVersion = CAST(ROUND((julianday('now') - 2440587.5)*86400000) AS INT)
                WHERE rowid = NEW.rowid;
            END
        ";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Version",
                table: "Accounts",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true,
                oldNullable: true);

            migrationBuilder.Sql(String.Format(_triggerQuery, "Accounts", "UPDATE"));
            migrationBuilder.Sql(String.Format(_triggerQuery, "Accounts", "INSERT"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Version",
                table: "Accounts",
                type: "BLOB",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldRowVersion: true,
                oldDefaultValue: 0L);

            migrationBuilder.Sql(String.Format(@"DROP TRIGGER Set{0}RowVersion{1}", "Accounts", "UPDATE"));
            migrationBuilder.Sql(String.Format(@"DROP TRIGGER Set{0}RowVersion{1}", "Accounts", "INSERT"));
        }
    }
}
