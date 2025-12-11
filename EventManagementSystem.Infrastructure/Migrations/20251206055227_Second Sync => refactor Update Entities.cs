using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementSystem.Infrastructure.Migrations
{
    public partial class SecondSyncrefactorUpdateEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationExpiresAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerifiedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMfaEnabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MfaSecret",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "EmailVerificationExpiresAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailVerificationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailVerifiedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsMfaEnabled",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MfaSecret",
                table: "AspNetUsers");

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
        }
    }
}
