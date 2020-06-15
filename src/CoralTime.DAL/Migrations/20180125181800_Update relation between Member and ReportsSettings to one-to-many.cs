using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.DAL.Migrations
{
    public partial class UpdaterelationbetweenMemberandReportsSettingstoonetomany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportsSettings_MemberId",
                table: "ReportsSettings");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ReportsSettings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportsSettings_MemberId",
                table: "ReportsSettings",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportsSettings_MemberId",
                table: "ReportsSettings");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ReportsSettings");

            migrationBuilder.CreateIndex(
                name: "IX_ReportsSettings_MemberId",
                table: "ReportsSettings",
                column: "MemberId",
                unique: true);
        }
    }
}
