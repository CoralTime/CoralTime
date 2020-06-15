using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoralTime.DAL.Migrations
{
    public partial class addrevertlinkfromMemberImageToMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberImages_MemberId",
                table: "MemberImages");

            migrationBuilder.CreateIndex(
                name: "IX_MemberImages_MemberId",
                table: "MemberImages",
                column: "MemberId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberImages_MemberId",
                table: "MemberImages");

            migrationBuilder.CreateIndex(
                name: "IX_MemberImages_MemberId",
                table: "MemberImages",
                column: "MemberId");
        }
    }
}
