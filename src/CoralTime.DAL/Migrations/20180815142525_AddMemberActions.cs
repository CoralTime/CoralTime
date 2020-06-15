using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.DAL.Migrations
{
    public partial class AddMemberActions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    ChangedObject = table.Column<string>(nullable: true),
                    Entity = table.Column<string>(nullable: true),
                    ChangedFields = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: false),
                    EntityId = table.Column<string>(nullable: true),
                    MemberId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberActions_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberActions_MemberId",
                table: "MemberActions",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberActions");
        }
    }
}
