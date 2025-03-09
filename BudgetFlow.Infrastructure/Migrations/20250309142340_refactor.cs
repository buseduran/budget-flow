using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_Users_UserId",
                table: "Portfolios");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Portfolios",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Portfolios_UserId",
                table: "Portfolios",
                newName: "IX_Portfolios_UserID");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Portfolios",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Investments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_Users_UserID",
                table: "Portfolios",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_Users_UserID",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Investments");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Portfolios",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Portfolios_UserID",
                table: "Portfolios",
                newName: "IX_Portfolios_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Portfolios",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_Users_UserId",
                table: "Portfolios",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
