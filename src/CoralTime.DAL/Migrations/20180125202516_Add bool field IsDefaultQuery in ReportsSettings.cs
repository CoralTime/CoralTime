using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.DAL.Migrations
{
    public partial class AddboolfieldIsDefaultQueryinReportsSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportsSettings_MemberId",
                table: "ReportsSettings");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultQuery",
                table: "ReportsSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ReportsSettings_MemberId_IsDefaultQuery",
                table: "ReportsSettings",
                columns: new[] { "MemberId", "IsDefaultQuery" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportsSettings_MemberId_IsDefaultQuery",
                table: "ReportsSettings");

            migrationBuilder.DropColumn(
                name: "IsDefaultQuery",
                table: "ReportsSettings");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsSettings_MemberId",
                table: "ReportsSettings",
                column: "MemberId");
        }
    }
}
