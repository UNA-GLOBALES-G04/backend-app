using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapp.Migrations
{
    public partial class UserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    fullName = table.Column<string>(type: "text", nullable: false),
                    legalDocumentID = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    birthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true),
                    profilePictureID = table.Column<string>(type: "text", nullable: false),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    isVerified = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
