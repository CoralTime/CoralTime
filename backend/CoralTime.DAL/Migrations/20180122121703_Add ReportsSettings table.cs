using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CoralTime.DAL.Migrations
{
    public partial class AddReportsSettingstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportsSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientIds = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: true),
                    DateTo = table.Column<DateTime>(nullable: true),
                    GroupById = table.Column<int>(nullable: true),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    MemberId = table.Column<int>(nullable: false),
                    MemberIds = table.Column<string>(nullable: true),
                    ProjectIds = table.Column<string>(nullable: true),
                    ShowColumnIds = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportsSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportsSettings_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportsSettings_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportsSettings_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportsSettings_CreatorId",
                table: "ReportsSettings",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsSettings_LastEditorUserId",
                table: "ReportsSettings",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsSettings_MemberId",
                table: "ReportsSettings",
                column: "MemberId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportsSettings");
        }
    }
}
