using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewPharmacy.Migrations
{
    /// <inheritdoc />
    public partial class ChatNewChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_MyAppUsers_MyAppUserId",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "MyAppUserId",
                table: "Chats",
                newName: "SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_MyAppUserId",
                table: "Chats",
                newName: "IX_Chats_SenderId");

            migrationBuilder.AddColumn<bool>(
                name: "IsResponse",
                table: "Chats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_MyAppUsers_SenderId",
                table: "Chats",
                column: "SenderId",
                principalTable: "MyAppUsers",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_MyAppUsers_SenderId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "IsResponse",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Chats",
                newName: "MyAppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_SenderId",
                table: "Chats",
                newName: "IX_Chats_MyAppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_MyAppUsers_MyAppUserId",
                table: "Chats",
                column: "MyAppUserId",
                principalTable: "MyAppUsers",
                principalColumn: "ID");
        }
    }
}
