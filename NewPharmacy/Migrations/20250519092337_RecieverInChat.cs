using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewPharmacy.Migrations
{
    /// <inheritdoc />
    public partial class RecieverInChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReceiverId",
                table: "Chats",
                type: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ReceiverId",
                table: "Chats",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_MyAppUsers_ReceiverId",
                table: "Chats",
                column: "ReceiverId",
                principalTable: "MyAppUsers",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_MyAppUsers_ReceiverId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_ReceiverId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Chats");
        }
    }
}
