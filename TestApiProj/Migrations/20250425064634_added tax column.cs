using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApiProj.Migrations
{
    /// <inheritdoc />
    public partial class addedtaxcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ItemTax",
                table: "Items",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemTax",
                table: "Items");
        }
    }
}
