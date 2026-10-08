using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewPharmacy.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToMyAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MyAppUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MyAppUsers");
        }
    }
}
