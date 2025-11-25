using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace medibook_API.Migrations
{
    /// <inheritdoc />
    public partial class ADDSENDERRECEIVERNOTIFICATIONS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_Usersuser_id",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_Usersuser_id",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Usersuser_id",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "reciever_user_id",
                table: "Notifications",
                type: "INT",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sender_user_id",
                table: "Notifications",
                type: "INT",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_reciever_user_id",
                table: "Notifications",
                column: "reciever_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_sender_user_id",
                table: "Notifications",
                column: "sender_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_RecieveNotification",
                table: "Notifications",
                column: "reciever_user_id",
                principalTable: "Users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_SenderNotification",
                table: "Notifications",
                column: "sender_user_id",
                principalTable: "Users",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_RecieveNotification",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_User_SenderNotification",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_reciever_user_id",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_sender_user_id",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "reciever_user_id",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "sender_user_id",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "Usersuser_id",
                table: "Notifications",
                type: "INT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Usersuser_id",
                table: "Notifications",
                column: "Usersuser_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_Usersuser_id",
                table: "Notifications",
                column: "Usersuser_id",
                principalTable: "Users",
                principalColumn: "user_id");
        }
    }
}
