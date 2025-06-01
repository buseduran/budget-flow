using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BudgetFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSummaryReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SummaryReports",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WalletID = table.Column<int>(type: "integer", nullable: false),
                    Analysis = table.Column<string>(type: "text", nullable: true),
                    AnalysisDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryReports", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SummaryReports_Wallets_WalletID",
                        column: x => x.WalletID,
                        principalTable: "Wallets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SummaryReports_WalletID",
                table: "SummaryReports",
                column: "WalletID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SummaryReports");
        }
    }
}
