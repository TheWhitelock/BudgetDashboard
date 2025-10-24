using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetDashboard.Data.Migrations
{
    /// <inheritdoc />
    public partial class BudgetItem_IsOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                table: "BudgetItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOptional",
                table: "BudgetItems");
        }
    }
}
