using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeinvestmentamount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Investments",
                newName: "UnitAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrencyAmount",
                table: "Investments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyAmount",
                table: "Investments");

            migrationBuilder.RenameColumn(
                name: "UnitAmount",
                table: "Investments",
                newName: "Amount");
        }
    }
}
