using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementSystem.Migrations
{
    public partial class SyncUpdateEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("b9c565ac-8c34-400b-a17f-6f1b475e7753"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("e5c6cf9f-90f7-4ccc-899a-a669d3c034e0"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EventSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "EventSchedules",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DownloadCount",
                table: "Certificates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDownloadedAt",
                table: "Certificates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventParticipants",
                columns: table => new
                {
                    EventParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    EventScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipants", x => x.EventParticipantId);
                    table.ForeignKey(
                        name: "FK_EventParticipants_EventSchedules_EventScheduleId",
                        column: x => x.EventScheduleId,
                        principalTable: "EventSchedules",
                        principalColumn: "EventScheduleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedAt", "Email", "IsActive", "Name", "Phone", "ProfilePicture", "UpdatedAt", "UserType" },
                values: new object[] { new Guid("1c775abe-6641-4aad-848d-062a605e3c6f"), null, new DateTime(2025, 12, 4, 9, 43, 29, 599, DateTimeKind.Utc).AddTicks(99), "jane.volunteer@eventms.com", true, "Jane Smith", "+0987654321", null, new DateTime(2025, 12, 4, 9, 43, 29, 599, DateTimeKind.Utc).AddTicks(100), 1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedAt", "Email", "IsActive", "Name", "Phone", "ProfilePicture", "UpdatedAt", "UserType" },
                values: new object[] { new Guid("7e08e42b-441d-4f58-8b26-aae415e3bbca"), null, new DateTime(2025, 12, 4, 9, 43, 29, 599, DateTimeKind.Utc).AddTicks(42), "admin@eventms.com", true, "System Administrator", null, null, new DateTime(2025, 12, 4, 9, 43, 29, 599, DateTimeKind.Utc).AddTicks(43), 0 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedAt", "Email", "IsActive", "Name", "Phone", "ProfilePicture", "UpdatedAt", "UserType" },
                values: new object[] { new Guid("a4ab86d3-458e-4856-a638-ef3c7e1b9fae"), null, new DateTime(2025, 12, 4, 9, 43, 29, 599, DateTimeKind.Utc).AddTicks(72), "john.speaker@eventms.com", true, "John Doe", "+1234567890", null, new DateTime(2025, 12, 4, 9, 43, 29, 599, DateTimeKind.Utc).AddTicks(72), 2 });

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_EventScheduleId_UserId",
                table: "EventParticipants",
                columns: new[] { "EventScheduleId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_UserId",
                table: "EventParticipants",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventParticipants");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("1c775abe-6641-4aad-848d-062a605e3c6f"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("7e08e42b-441d-4f58-8b26-aae415e3bbca"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("a4ab86d3-458e-4856-a638-ef3c7e1b9fae"));

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EventSchedules");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "EventSchedules");

            migrationBuilder.DropColumn(
                name: "DownloadCount",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "LastDownloadedAt",
                table: "Certificates");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedAt", "Email", "Name", "Phone", "ProfilePicture", "UpdatedAt", "UserType" },
                values: new object[] { new Guid("b9c565ac-8c34-400b-a17f-6f1b475e7753"), null, new DateTime(2025, 12, 2, 11, 1, 43, 386, DateTimeKind.Utc).AddTicks(6274), "speaker@example.com", "John Speaker", null, null, new DateTime(2025, 12, 2, 11, 1, 43, 386, DateTimeKind.Utc).AddTicks(6275), 2 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedAt", "Email", "Name", "Phone", "ProfilePicture", "UpdatedAt", "UserType" },
                values: new object[] { new Guid("e5c6cf9f-90f7-4ccc-899a-a669d3c034e0"), null, new DateTime(2025, 12, 2, 11, 1, 43, 386, DateTimeKind.Utc).AddTicks(6238), "admin@example.com", "Admin User", null, null, new DateTime(2025, 12, 2, 11, 1, 43, 386, DateTimeKind.Utc).AddTicks(6239), 0 });
        }
    }
}
