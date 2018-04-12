using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoralTime.DAL.Migrations
{
    public partial class RenameMemberAvatartoMemberImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberAvatars");

            migrationBuilder.CreateTable(
                name: "MemberImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ByteArrayAvatar = table.Column<byte[]>(nullable: true),
                    ByteArrayIcon = table.Column<byte[]>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    FileNameImage = table.Column<string>(maxLength: 200, nullable: true),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    MemberId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberImages_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberImages_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberImages_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberImages_CreatorId",
                table: "MemberImages",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberImages_LastEditorUserId",
                table: "MemberImages",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberImages_MemberId",
                table: "MemberImages",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberImages");

            migrationBuilder.CreateTable(
                name: "MemberAvatars",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AvatarFile = table.Column<byte[]>(nullable: true),
                    AvatarFileName = table.Column<string>(maxLength: 200, nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    MemberId = table.Column<int>(nullable: false),
                    ThumbnailFile = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberAvatars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberAvatars_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberAvatars_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberAvatars_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberAvatars_CreatorId",
                table: "MemberAvatars",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAvatars_LastEditorUserId",
                table: "MemberAvatars",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAvatars_MemberId",
                table: "MemberAvatars",
                column: "MemberId");
        }
    }
}
