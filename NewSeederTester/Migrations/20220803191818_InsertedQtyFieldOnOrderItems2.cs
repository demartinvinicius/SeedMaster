using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewSeederTester.Migrations
{
    public partial class InsertedQtyFieldOnOrderItems2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Orders");

            migrationBuilder.AddColumn<long>(
                name: "Qty",
                table: "OrdersItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Qty",
                table: "OrdersItems");

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
