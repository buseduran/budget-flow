using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDbRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserID",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Wallets_WalletID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_WalletID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserID",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "WalletID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WalletID",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Categories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_WalletID",
                table: "Users",
                column: "WalletID");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserID",
                table: "Categories",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserID",
                table: "Categories",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Wallets_WalletID",
                table: "Users",
                column: "WalletID",
                principalTable: "Wallets",
                principalColumn: "ID");
        }
    }
}
