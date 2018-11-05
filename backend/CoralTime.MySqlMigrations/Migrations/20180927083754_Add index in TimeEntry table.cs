using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.MySqlMigrations.Migrations
{
    public partial class AddindexinTimeEntrytable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_MemberId_Date",
                table: "TimeEntries",
                columns: new[] { "MemberId", "Date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeEntries_MemberId_Date",
                table: "TimeEntries");
        }
    }
}
