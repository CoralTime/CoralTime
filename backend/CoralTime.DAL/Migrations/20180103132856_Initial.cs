using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CoralTime.DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    IsManager = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "UserForgotPassRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    RefreshToken = table.Column<string>(nullable: false),
                    UserForgotPassRequestUid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserForgotPassRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersistedGrants",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Expiration = table.Column<DateTime>(nullable: true),
                    SubjectId = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersistedGrants", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Email = table.Column<string>(maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    DateFormatId = table.Column<int>(nullable: false),
                    DefaultProjectId = table.Column<int>(nullable: false),
                    DefaultTaskId = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(maxLength: 200, nullable: false),
                    IsWeeklyTimeEntryUpdatesSend = table.Column<bool>(nullable: false),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    SendEmailDays = table.Column<int>(nullable: true),
                    SendEmailTime = table.Column<int>(nullable: false),
                    TimeFormat = table.Column<int>(nullable: false),
                    TimeZone = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    WeekStart = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Members_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Members_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(nullable: true),
                    Color = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    DaysBeforeStopEditTimeEntries = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsActiveBeforeArchiving = table.Column<bool>(nullable: false),
                    IsNotificationEnabled = table.Column<bool>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    IsTimeLockEnabled = table.Column<bool>(nullable: false),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LockPeriod = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    NotificationDay = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "MemberProjectRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    MemberId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberProjectRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberProjectRoles_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberProjectRoles_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberProjectRoles_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberProjectRoles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberProjectRoles_ProjectRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ProjectRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Color = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    ProjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTypes_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskTypes_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskTypes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    IsEditable = table.Column<bool>(nullable: false),
                    IsFromToShow = table.Column<bool>(nullable: false),
                    IsLocked = table.Column<bool>(nullable: false),
                    LastEditorUserId = table.Column<string>(nullable: true),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    MemberId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    PlannedTime = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    TaskTypesId = table.Column<int>(nullable: false),
                    Time = table.Column<int>(nullable: false),
                    TimeFrom = table.Column<int>(nullable: false),
                    TimeTimerStart = table.Column<int>(nullable: false),
                    TimeTo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeEntries_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeEntries_AspNetUsers_LastEditorUserId",
                        column: x => x.LastEditorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeEntries_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeEntries_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeEntries_TaskTypes_TaskTypesId",
                        column: x => x.TaskTypesId,
                        principalTable: "TaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CreatorId",
                table: "Clients",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_LastEditorUserId",
                table: "Clients",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_CreatorId",
                table: "Members",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_LastEditorUserId",
                table: "Members",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_UserId",
                table: "Members",
                column: "UserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_MemberProjectRoles_CreatorId",
                table: "MemberProjectRoles",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberProjectRoles_LastEditorUserId",
                table: "MemberProjectRoles",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberProjectRoles_ProjectId",
                table: "MemberProjectRoles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberProjectRoles_RoleId",
                table: "MemberProjectRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberProjectRoles_MemberId_ProjectId",
                table: "MemberProjectRoles",
                columns: new[] { "MemberId", "ProjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ClientId",
                table: "Projects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatorId",
                table: "Projects",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LastEditorUserId",
                table: "Projects",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name",
                table: "Projects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskTypes_CreatorId",
                table: "TaskTypes",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTypes_LastEditorUserId",
                table: "TaskTypes",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTypes_ProjectId",
                table: "TaskTypes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_CreatorId",
                table: "TimeEntries",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_LastEditorUserId",
                table: "TimeEntries",
                column: "LastEditorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_MemberId",
                table: "TimeEntries",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_ProjectId",
                table: "TimeEntries",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_TaskTypesId",
                table: "TimeEntries",
                column: "TaskTypesId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberAvatars");

            migrationBuilder.DropTable(
                name: "MemberProjectRoles");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "TimeEntries");

            migrationBuilder.DropTable(
                name: "UserForgotPassRequests");

            migrationBuilder.DropTable(
                name: "PersistedGrants");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ProjectRoles");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "TaskTypes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
