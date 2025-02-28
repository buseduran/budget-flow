using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class categorytypetoentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryType",
                table: "Entries",
                newName: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_CategoryID",
                table: "Entries",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_Categories_CategoryID",
                table: "Entries",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_Categories_CategoryID",
                table: "Entries");

            migrationBuilder.DropIndex(
                name: "IX_Entries_CategoryID",
                table: "Entries");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "Entries",
                newName: "CategoryType");
        }
    }
}
