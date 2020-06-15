using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.DAL.Migrations
{
    public partial class AddVSTStables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VstsProjects",
                columns: table => new
                {
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: false),
                    VstsProjectId = table.Column<string>(nullable: true),
                    VstsProjectName = table.Column<string>(nullable: true),
                    VstsCompanyUrl = table.Column<string>(nullable: true),
                    VstsPat = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VstsProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VstsProjects_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VstsProjects_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VstsProjects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VstsUsers",
                columns: table => new
                {
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    VstsUserId = table.Column<string>(nullable: true),
                    VstsUserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VstsUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VstsUsers_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VstsUsers_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VstsUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VstsProjects_CreatorId",
                table: "VstsProjects",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_VstsProjects_LastEditorUserId",
                table: "VstsProjects",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VstsProjects_ProjectId",
                table: "VstsProjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_VstsUsers_CreatorId",
                table: "VstsUsers",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_VstsUsers_LastEditorUserId",
                table: "VstsUsers",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VstsUsers_UserId",
                table: "VstsUsers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VstsProjects");

            migrationBuilder.DropTable(
                name: "VstsUsers");
        }
    }
}
