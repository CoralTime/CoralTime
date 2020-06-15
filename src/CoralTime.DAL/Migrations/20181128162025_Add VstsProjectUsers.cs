using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.DAL.Migrations
{
    public partial class AddVstsProjectUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VstsUsers_AspNetUsers_UserId",
                table: "VstsUsers");

            migrationBuilder.DropIndex(
                name: "IX_VstsUsers_UserId",
                table: "VstsUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "VstsUsers");

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "VstsUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VstsProjectUsers",
                columns: table => new
                {
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VstsUserId = table.Column<int>(nullable: false),
                    VstsProjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VstsProjectUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VstsProjectUsers_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VstsProjectUsers_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VstsProjectUsers_VstsProjects_VstsProjectId",
                        column: x => x.VstsProjectId,
                        principalTable: "VstsProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VstsProjectUsers_VstsUsers_VstsUserId",
                        column: x => x.VstsUserId,
                        principalTable: "VstsUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VstsUsers_MemberId",
                table: "VstsUsers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_VstsProjectUsers_CreatorId",
                table: "VstsProjectUsers",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_VstsProjectUsers_LastEditorUserId",
                table: "VstsProjectUsers",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VstsProjectUsers_VstsProjectId",
                table: "VstsProjectUsers",
                column: "VstsProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_VstsProjectUsers_VstsUserId_VstsProjectId",
                table: "VstsProjectUsers",
                columns: new[] { "VstsUserId", "VstsProjectId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VstsUsers_Members_MemberId",
                table: "VstsUsers",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VstsUsers_Members_MemberId",
                table: "VstsUsers");

            migrationBuilder.DropTable(
                name: "VstsProjectUsers");

            migrationBuilder.DropIndex(
                name: "IX_VstsUsers_MemberId",
                table: "VstsUsers");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "VstsUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "VstsUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VstsUsers_UserId",
                table: "VstsUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VstsUsers_AspNetUsers_UserId",
                table: "VstsUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
