using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapp.Migrations
{
    public partial class order_description : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "Orders");
        }
    }
}
