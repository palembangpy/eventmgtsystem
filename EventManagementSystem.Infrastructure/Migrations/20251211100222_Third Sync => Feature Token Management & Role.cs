using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementSystem.Infrastructure.Migrations
{
    public partial class ThirdSyncFeatureTokenManagementRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("0a0a96ec-2371-4cfb-8272-4b05c705fa1d"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("27f5c867-8989-4ae7-8b7b-e13a5d12af51"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("c2a03d89-2c3e-4a15-a0e2-3ebd55fe013d"));

            migrationBuilder.AddColumn<int>(
                name: "systemRole",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tokenizers",
                columns: table => new
                {
                    TokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    TokenName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllowedEndpoints = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokenizers", x => x.TokenId);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedAt", "Email", "IsActive", "Name", "Phone", "ProfilePicture", "UpdatedAt", "UserType" },
                values: new object[] { new Guid("258764ef-38b4-4cdc-a35e-c2a1c3d0a954"), null, new DateTime(2025, 12, 11, 10, 2, 22, 541, DateTimeKind.Utc).AddTicks(6503), "john.speaker@eventms.com", true, "John Doe", "+1234567890", null, new DateTime(2025, 12, 11, 10, 2, 22, 541, DateTimeKind.Utc).AddTicks(6503), 3 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedAt", "Email", "IsActive", "Name", "Phone", "ProfilePicture", "UpdatedAt", "UserType" },
                values: new object[] { new Guid("2be8fea0-6948-4b90-a0f2-98cba73ea6d0"), null, new DateTime(2025, 12, 11, 10, 2, 22, 541, DateTimeKind.Utc).AddTicks(6483), "admin@eventms.com", true, "System Administrator", null, null, new DateTime(2025, 12, 11, 10, 2, 22, 541, DateTimeKind.Utc).AddTicks(6483), 1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedAt", "Email", "IsActive", "Name", "Phone", "ProfilePicture", "UpdatedAt", "UserType" },
                values: new object[] { new Guid("9dbd641d-b0a7-4b52-b3cb-d6c48d04e804"), null, new DateTime(2025, 12, 11, 10, 2, 22, 541, DateTimeKind.Utc).AddTicks(6522), "jane.volunteer@eventms.com", true, "Jane Smith", "+0987654321", null, new DateTime(2025, 12, 11, 10, 2, 22, 541, DateTimeKind.Utc).AddTicks(6523), 2 });

            migrationBuilder.CreateIndex(
                name: "IX_Tokenizers_TokenHash",
                table: "Tokenizers",
                column: "TokenHash",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tokenizers");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("258764ef-38b4-4cdc-a35e-c2a1c3d0a954"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("2be8fea0-6948-4b90-a0f2-98cba73ea6d0"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("9dbd641d-b0a7-4b52-b3cb-d6c48d04e804"));

            migrationBuilder.DropColumn(
                name: "systemRole",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedAt", "Email", "IsActive", "Name", "Phone", "ProfilePicture", "UpdatedAt", "UserType" },
                values: new object[] { new Guid("0a0a96ec-2371-4cfb-8272-4b05c705fa1d"), null, new DateTime(2025, 12, 6, 5, 52, 26, 883, DateTimeKind.Utc).AddTicks(4575), "john.speaker@eventms.com", true, "John Doe", "+1234567890", null, new DateTime(2025, 12, 6, 5, 52, 26, 883, DateTimeKind.Utc).AddTicks(4575), 2 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedAt", "Email", "IsActive", "Name", "Phone", "ProfilePicture", "UpdatedAt", "UserType" },
                values: new object[] { new Guid("27f5c867-8989-4ae7-8b7b-e13a5d12af51"), null, new DateTime(2025, 12, 6, 5, 52, 26, 883, DateTimeKind.Utc).AddTicks(4551), "admin@eventms.com", true, "System Administrator", null, null, new DateTime(2025, 12, 6, 5, 52, 26, 883, DateTimeKind.Utc).AddTicks(4552), 0 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedAt", "Email", "IsActive", "Name", "Phone", "ProfilePicture", "UpdatedAt", "UserType" },
                values: new object[] { new Guid("c2a03d89-2c3e-4a15-a0e2-3ebd55fe013d"), null, new DateTime(2025, 12, 6, 5, 52, 26, 883, DateTimeKind.Utc).AddTicks(4597), "jane.volunteer@eventms.com", true, "Jane Smith", "+0987654321", null, new DateTime(2025, 12, 6, 5, 52, 26, 883, DateTimeKind.Utc).AddTicks(4598), 1 });
        }
    }
}
