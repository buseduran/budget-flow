using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BudgetFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Users_UserId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Wallets");

            migrationBuilder.AddColumn<int>(
                name: "WalletID",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WalletID",
                table: "Entries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserWallets",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WalletID = table.Column<int>(type: "integer", nullable: false),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWallets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserWallets_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWallets_Wallets_WalletID",
                        column: x => x.WalletID,
                        principalTable: "Wallets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_WalletID",
                table: "Users",
                column: "WalletID");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_WalletID",
                table: "Entries",
                column: "WalletID");

            migrationBuilder.CreateIndex(
                name: "IX_UserWallets_UserID",
                table: "UserWallets",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserWallets_WalletID",
                table: "UserWallets",
                column: "WalletID");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_Wallets_WalletID",
                table: "Entries",
                column: "WalletID",
                principalTable: "Wallets",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Wallets_WalletID",
                table: "Users",
                column: "WalletID",
                principalTable: "Wallets",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_Wallets_WalletID",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Wallets_WalletID",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserWallets");

            migrationBuilder.DropIndex(
                name: "IX_Users_WalletID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Entries_WalletID",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "WalletID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WalletID",
                table: "Entries");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Wallets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Users_UserId",
                table: "Wallets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
