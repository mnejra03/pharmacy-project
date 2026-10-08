using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NewPharmacy.Data;

#nullable disable

namespace NewPharmacy.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260323000100_RedesignWishListModel")]
    public partial class RedesignWishListModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [WishListDetails]");

            migrationBuilder.DropForeignKey(
                name: "FK_WishListDetails_MyAppUsers_MyAppUserId",
                table: "WishListDetails");

            migrationBuilder.DropIndex(
                name: "IX_WishListDetails_MyAppUserId",
                table: "WishListDetails");

            migrationBuilder.DropIndex(
                name: "IX_WishLists_MyAppUserId",
                table: "WishLists");

            migrationBuilder.DropColumn(
                name: "MyAppUserId",
                table: "WishListDetails");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "WishListDetails");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedAt",
                table: "WishListDetails",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "WishListDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WishLists_MyAppUserId",
                table: "WishLists",
                column: "MyAppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WishListDetails_ProductId",
                table: "WishListDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WishListDetails_WishListId_ProductId",
                table: "WishListDetails",
                columns: new[] { "WishListId", "ProductId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WishListDetails_Products_ProductId",
                table: "WishListDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishListDetails_Products_ProductId",
                table: "WishListDetails");

            migrationBuilder.DropIndex(
                name: "IX_WishListDetails_ProductId",
                table: "WishListDetails");

            migrationBuilder.DropIndex(
                name: "IX_WishListDetails_WishListId_ProductId",
                table: "WishListDetails");

            migrationBuilder.DropIndex(
                name: "IX_WishLists_MyAppUserId",
                table: "WishLists");

            migrationBuilder.DropColumn(
                name: "AddedAt",
                table: "WishListDetails");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "WishListDetails");

            migrationBuilder.AddColumn<int>(
                name: "MyAppUserId",
                table: "WishListDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "WishListDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WishLists_MyAppUserId",
                table: "WishLists",
                column: "MyAppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WishListDetails_MyAppUserId",
                table: "WishListDetails",
                column: "MyAppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WishListDetails_MyAppUsers_MyAppUserId",
                table: "WishListDetails",
                column: "MyAppUserId",
                principalTable: "MyAppUsers",
                principalColumn: "ID");
        }
    }
}
