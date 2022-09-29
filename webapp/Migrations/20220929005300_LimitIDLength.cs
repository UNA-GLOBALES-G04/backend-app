using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapp.Migrations
{
    public partial class LimitIDLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "UserProfiles",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldMaxLength: 36);
        }
    }
}
