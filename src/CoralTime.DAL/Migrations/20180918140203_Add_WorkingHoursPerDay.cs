using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.DAL.Migrations
{
    public partial class Add_WorkingHoursPerDay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkingHoursPerDay",
                table: "Members",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "MemberActions",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkingHoursPerDay",
                table: "Members");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "MemberActions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
