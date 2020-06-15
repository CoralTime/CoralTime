using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoralTime.DAL.Migrations
{
    public partial class UpdatetypeforGroupByIdinReportsSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GroupById",
                table: "ReportsSettings",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GroupById",
                table: "ReportsSettings",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
