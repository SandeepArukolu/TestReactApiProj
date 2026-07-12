using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApiProj.Migrations
{
    /// <inheritdoc />
    public partial class priceColumnadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ItemPrice",
                table: "Items",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemPrice",
                table: "Items");
        }
    }
}
