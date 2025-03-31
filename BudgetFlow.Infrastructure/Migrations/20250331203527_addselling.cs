using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addselling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PurchasePrice",
                table: "Investments",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "PurchaseDate",
                table: "Investments",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "CurrentPrice",
                table: "Assets",
                newName: "SellPrice");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Investments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyPrice",
                table: "Assets",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "BuyPrice",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Investments",
                newName: "PurchasePrice");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Investments",
                newName: "PurchaseDate");

            migrationBuilder.RenameColumn(
                name: "SellPrice",
                table: "Assets",
                newName: "CurrentPrice");
        }
    }
}
