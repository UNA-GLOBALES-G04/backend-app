using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapp.Migrations
{
    public partial class UniqueConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_email",
                table: "UserProfiles",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_legalDocumentID",
                table: "UserProfiles",
                column: "legalDocumentID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_profilePictureID",
                table: "UserProfiles",
                column: "profilePictureID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_email",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_legalDocumentID",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_profilePictureID",
                table: "UserProfiles");
        }
    }
}
