using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryToUseWalletID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserID",
                table: "Categories");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Categories",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "WalletID",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_WalletID",
                table: "Categories",
                column: "WalletID");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserID",
                table: "Categories",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Wallets_WalletID",
                table: "Categories",
                column: "WalletID",
                principalTable: "Wallets",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserID",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Wallets_WalletID",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_WalletID",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "WalletID",
                table: "Categories");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserID",
                table: "Categories",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
