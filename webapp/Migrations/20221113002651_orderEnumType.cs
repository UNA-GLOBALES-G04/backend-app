using Microsoft.EntityFrameworkCore.Migrations;
using webapp.model;

#nullable disable

namespace webapp.Migrations
{
    public partial class orderEnumType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Orders");

            migrationBuilder.AddColumn<Order.OrderStatus>(
                name: "current_status",
                table: "Orders",
                type: "order_status",
                nullable: false,
                defaultValue: Order.OrderStatus.PENDING);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "current_status",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
