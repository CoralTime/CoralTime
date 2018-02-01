using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.DAL.Migrations
{
    public partial class AddboolIsCurrentQueryinreportssettingsandaddittouniquekey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportsSettings_MemberId_IsDefaultQuery_QueryName",
                table: "ReportsSettings");

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrentQuery",
                table: "ReportsSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ReportsSettings_MemberId_IsDefaultQuery_IsCurrentQuery_QueryName",
                table: "ReportsSettings",
                columns: new[] { "MemberId", "IsDefaultQuery", "IsCurrentQuery", "QueryName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportsSettings_MemberId_IsDefaultQuery_IsCurrentQuery_QueryName",
                table: "ReportsSettings");

            migrationBuilder.DropColumn(
                name: "IsCurrentQuery",
                table: "ReportsSettings");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsSettings_MemberId_IsDefaultQuery_QueryName",
                table: "ReportsSettings",
                columns: new[] { "MemberId", "IsDefaultQuery", "QueryName" },
                unique: true);
        }
    }
}
