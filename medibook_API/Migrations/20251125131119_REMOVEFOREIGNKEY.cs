using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace medibook_API.Migrations
{
    /// <inheritdoc />
    public partial class REMOVEFOREIGNKEY : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Notification",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_user_id",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "user_id",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "user_id",
                table: "Notifications",
                type: "INT",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_user_id",
                table: "Notifications",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Notification",
                table: "Notifications",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id");
        }
    }
}
