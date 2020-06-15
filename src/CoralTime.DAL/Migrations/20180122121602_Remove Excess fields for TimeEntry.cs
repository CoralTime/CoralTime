using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.DAL.Migrations
{
    public partial class RemoveExcessfieldsforTimeEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEditable",
                table: "TimeEntries");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "TimeEntries");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TimeEntries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEditable",
                table: "TimeEntries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "TimeEntries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TimeEntries",
                maxLength: 200,
                nullable: true);
        }
    }
}
