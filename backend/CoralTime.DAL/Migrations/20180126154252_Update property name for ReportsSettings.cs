using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.DAL.Migrations
{
    public partial class UpdatepropertynameforReportsSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShowColumnIds",
                table: "ReportsSettings",
                newName: "FilterShowColumnIds");

            migrationBuilder.RenameColumn(
                name: "ProjectIds",
                table: "ReportsSettings",
                newName: "FilterProjectIds");

            migrationBuilder.RenameColumn(
                name: "MemberIds",
                table: "ReportsSettings",
                newName: "FilterMemberIds");

            migrationBuilder.RenameColumn(
                name: "ClientIds",
                table: "ReportsSettings",
                newName: "FilterClientIds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilterShowColumnIds",
                table: "ReportsSettings",
                newName: "ShowColumnIds");

            migrationBuilder.RenameColumn(
                name: "FilterProjectIds",
                table: "ReportsSettings",
                newName: "ProjectIds");

            migrationBuilder.RenameColumn(
                name: "FilterMemberIds",
                table: "ReportsSettings",
                newName: "MemberIds");

            migrationBuilder.RenameColumn(
                name: "FilterClientIds",
                table: "ReportsSettings",
                newName: "ClientIds");
        }
    }
}
