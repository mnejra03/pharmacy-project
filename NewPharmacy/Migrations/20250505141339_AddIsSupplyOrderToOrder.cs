using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewPharmacy.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSupplyOrderToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSupplyOrder",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSupplyOrder",
                table: "Orders");
        }
    }
}
