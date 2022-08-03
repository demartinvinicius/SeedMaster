using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewSeederTester.Migrations
{
    public partial class FixedSuppliersName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Supliers_SuplierId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Supliers",
                table: "Supliers");

            migrationBuilder.RenameTable(
                name: "Supliers",
                newName: "Suppliers");

            migrationBuilder.RenameColumn(
                name: "SuplierId",
                table: "Products",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_SuplierId",
                table: "Products",
                newName: "IX_Products_SupplierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers_SupplierId",
                table: "Products",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_SupplierId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers");

            migrationBuilder.RenameTable(
                name: "Suppliers",
                newName: "Supliers");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "Products",
                newName: "SuplierId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                newName: "IX_Products_SuplierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Supliers",
                table: "Supliers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Supliers_SuplierId",
                table: "Products",
                column: "SuplierId",
                principalTable: "Supliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
