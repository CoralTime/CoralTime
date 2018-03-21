using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoralTime.DAL.Migrations
{
    public partial class RenameTimeValuesforTimeEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlannedTime",
                table: "TimeEntries",
                newName: "TimeEstimated");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "TimeEntries",
                newName: "TimeActual");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeEstimated",
                table: "TimeEntries",
                newName: "PlannedTime");

            migrationBuilder.RenameColumn(
                name: "TimeActual",
                table: "TimeEntries",
                newName: "Time");
        }
    }
}
