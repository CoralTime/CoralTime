using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.MySqlMigrations.Migrations
{
    public partial class AddWorkItemIdinTimeEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkItemId",
                table: "TimeEntries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkItemId",
                table: "TimeEntries");
        }
    }
}
