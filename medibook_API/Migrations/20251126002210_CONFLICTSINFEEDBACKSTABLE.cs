using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace medibook_API.Migrations
{
    /// <inheritdoc />
    public partial class CONFLICTSINFEEDBACKSTABLE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "create_date",
                table: "FeedBacks",
                newName: "feedback_date");

            migrationBuilder.RenameColumn(
                name: "appointment_date",
                table: "FeedBacks",
                newName: "reply_date");

            migrationBuilder.AlterColumn<string>(
                name: "doctor_reply",
                table: "FeedBacks",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldUnicode: false,
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "reply_date",
                table: "FeedBacks",
                type: "datetime2(0)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(0)");

            migrationBuilder.AlterColumn<int>(
                name: "room_id",
                table: "Appointments",
                type: "INT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INT");

            migrationBuilder.AlterColumn<int>(
                name: "nurse_id",
                table: "Appointments",
                type: "INT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reply_date",
                table: "FeedBacks",
                newName: "appointment_date");

            migrationBuilder.RenameColumn(
                name: "feedback_date",
                table: "FeedBacks",
                newName: "create_date");

            migrationBuilder.AlterColumn<string>(
                name: "doctor_reply",
                table: "FeedBacks",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldUnicode: false,
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "appointment_date",
                table: "FeedBacks",
                type: "datetime2(0)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2(0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "room_id",
                table: "Appointments",
                type: "INT",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "nurse_id",
                table: "Appointments",
                type: "INT",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INT",
                oldNullable: true);
        }
    }
}
