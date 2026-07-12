using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApiProj.Migrations
{
    /// <inheritdoc />
    public partial class DiscountColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ItemDiscount",
                table: "Items",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemDiscount",
                table: "Items");
        }
    }
}
