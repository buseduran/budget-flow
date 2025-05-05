using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInitialAndCurrentBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "InitialBalance",
            table: "Wallets");

            migrationBuilder.DropColumn(
                name: "CurrentBalance",
                table: "Wallets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
            name: "InitialBalance",
            table: "Wallets",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBalance",
                table: "Wallets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
