using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.DAL.Migrations
{
    public partial class UpdatepropertynameforReportsSettingsfromNametoQueryName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportsSettings_MemberId_IsDefaultQuery",
                table: "ReportsSettings");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ReportsSettings",
                newName: "QueryName");

            migrationBuilder.AlterColumn<string>(
                name: "QueryName",
                table: "ReportsSettings",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportsSettings_MemberId_IsDefaultQuery_QueryName",
                table: "ReportsSettings",
                columns: new[] { "MemberId", "IsDefaultQuery", "QueryName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportsSettings_MemberId_IsDefaultQuery_QueryName",
                table: "ReportsSettings");

            migrationBuilder.RenameColumn(
                name: "QueryName",
                table: "ReportsSettings",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ReportsSettings",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportsSettings_MemberId_IsDefaultQuery",
                table: "ReportsSettings",
                columns: new[] { "MemberId", "IsDefaultQuery" },
                unique: true);
        }
    }
}
