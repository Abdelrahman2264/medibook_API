using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace medibook_API.Migrations
{
    /// <inheritdoc />
    public partial class INITMIGRATIONFORDATABASE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    report_id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    report_pdf = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    report_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.report_id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("ROLEID_PK", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    room_id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    room_name = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    room_type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("ROOMID_PK", x => x.room_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    gender = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    mitrial_status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    mobile_phone = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    role_id = table.Column<int>(type: "INT", nullable: false),
                    profile_image = table.Column<byte[]>(type: "VARBINARY(MAX)", maxLength: 5242880, nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "(sysdatetime())"),
                    date_of_birth = table.Column<DateTime>(type: "datetime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("USERUID_PK", x => x.user_id);
                    table.CheckConstraint("CK_Gender_Values", "gender IN ('Male', 'Female')");
                    table.ForeignKey(
                        name: "FK_User_Role",
                        column: x => x.role_id,
                        principalTable: "Roles",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    doctor_id = table.Column<int>(type: "INT", nullable: false),
                    bio = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    specialization = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    type = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    experience_years = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("DOCTORID_PK", x => x.doctor_id);
                    table.CheckConstraint("CK_ExperienceYears_NonNegative", "experience_years >= 0");
                    table.ForeignKey(
                        name: "FK_User_Doctor",
                        column: x => x.doctor_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    action = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    log_type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    log_date = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "(sysdatetime())"),
                    description = table.Column<string>(type: "varchar(max)", unicode: false, maxLength: 2147483647, nullable: false),
                    user_id = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LOGID_PK", x => x.log_id);
                    table.ForeignKey(
                        name: "FK_Logs_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "INT", nullable: false),
                    message = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "(sysdatetime())"),
                    read_date = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    user_id = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("NOTIFICATIONID_PK", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_User_Notification",
                        column: x => x.notification_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Nurses",
                columns: table => new
                {
                    nurse_id = table.Column<int>(type: "INT", nullable: false),
                    bio = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    user_id = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("NURSEID_PK", x => x.nurse_id);
                    table.ForeignKey(
                        name: "FK_User_Nurse",
                        column: x => x.nurse_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    appointment_id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    appointment_date = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    notes = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    medicine = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "(sysdatetime())"),
                    patient_id = table.Column<int>(type: "INT", nullable: false),
                    doctor_id = table.Column<int>(type: "INT", nullable: false),
                    nurse_id = table.Column<int>(type: "INT", nullable: false),
                    room_id = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("APPOINTMENTID_PK", x => x.appointment_id);
                    table.ForeignKey(
                        name: "FK_Doctor_Appointment",
                        column: x => x.doctor_id,
                        principalTable: "Doctors",
                        principalColumn: "doctor_id");
                    table.ForeignKey(
                        name: "FK_Nurse_Appointment",
                        column: x => x.nurse_id,
                        principalTable: "Nurses",
                        principalColumn: "nurse_id");
                    table.ForeignKey(
                        name: "FK_Patient_Appointment",
                        column: x => x.patient_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Room_Appointment",
                        column: x => x.room_id,
                        principalTable: "Rooms",
                        principalColumn: "room_id");
                });

            migrationBuilder.CreateTable(
                name: "FeedBacks",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    comment = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    rate = table.Column<int>(type: "int", nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "(sysdatetime())"),
                    doctor_reply = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    appointment_date = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    is_favourite = table.Column<bool>(type: "bit", nullable: false),
                    patient_id = table.Column<int>(type: "INT", nullable: false),
                    doctor_id = table.Column<int>(type: "INT", nullable: false),
                    appointment_id = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("FEEDBACKID_PK", x => x.feedback_id);
                    table.CheckConstraint("CK_Rate_Range", "rate >= 1 AND rate <= 5");
                    table.ForeignKey(
                        name: "FK_Appointment_FeedBack",
                        column: x => x.appointment_id,
                        principalTable: "Appointments",
                        principalColumn: "appointment_id");
                    table.ForeignKey(
                        name: "FK_Doctor_FeedBack",
                        column: x => x.doctor_id,
                        principalTable: "Doctors",
                        principalColumn: "doctor_id");
                    table.ForeignKey(
                        name: "FK_Patient_FeedBack",
                        column: x => x.patient_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_doctor_id",
                table: "Appointments",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_nurse_id",
                table: "Appointments",
                column: "nurse_id");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_patient_id",
                table: "Appointments",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_room_id",
                table: "Appointments",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedBacks_appointment_id",
                table: "FeedBacks",
                column: "appointment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedBacks_doctor_id",
                table: "FeedBacks",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedBacks_patient_id",
                table: "FeedBacks",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_user_id",
                table: "Logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ROOMNAME_UQ",
                table: "Rooms",
                column: "room_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_role_id",
                table: "Users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "USEREMAIL_UQ",
                table: "Users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "USERPHONE_UQ",
                table: "Users",
                column: "mobile_phone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedBacks");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Nurses");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
