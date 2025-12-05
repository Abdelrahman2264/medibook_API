using medibook_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace medibook_API.Data
{
    public class Medibook_Context : DbContext
    {
        public Medibook_Context(DbContextOptions<Medibook_Context> options) : base(options)
        {

        }
        public virtual DbSet<Users> Users { get; set; } = null!;
        public virtual DbSet<Doctors> Doctors { get; set; } = null!;
        public virtual DbSet<Nurses> Nurses { get; set; } = null!;
        public virtual DbSet<Rooms> Rooms { get; set; } = null!;
        public virtual DbSet<Logs> Logs { get; set; } = null!;
        public virtual DbSet<Roles> Roles { get; set; } = null!;
        public virtual DbSet<Appointments> Appointments { get; set; } = null!;
        public virtual DbSet<FeedBacks> FeedBacks { get; set; } = null!;
        public virtual DbSet<Reports> Reports { get; set; } = null!;
        public virtual DbSet<Notifications> Notifications { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.user_id).HasName("USERID_PK");
                entity.Property(e => e.user_id)
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => e.email, "EMAIL_UQ")
                  .IsUnique();
                entity.HasIndex(e => e.mobile_phone, "MOBILEPHONE_UQ")
                 .IsUnique();

                entity.Property(e => e.first_name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("first_name")
                    .IsRequired();

                entity.Property(e => e.last_name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("last_name")
                    .IsRequired();
                entity.Property(e => e.role_id)
                .IsRequired();

                entity.Property(e => e.email)
                     .HasMaxLength(150)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("email")
                     .HasConversion(
                         v => v,
                         v => v.ToLower()
                     )
                     .HasAnnotation("RegularExpression",
                         @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

                entity.Property(e => e.mobile_phone)
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .IsRequired()
                        .HasColumnName("mobile_phone");

                entity.Property(e => e.password_hash)
                        .IsUnicode(false)
                        .IsRequired()
                        .HasColumnName("password_hash");

                entity.Property(e => e.is_active)
                .IsRequired()
                .HasColumnType<bool>("bit")
                .HasColumnName("is_active");

                entity.Property(e => e.mitrial_status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsRequired()
                    .HasColumnName("mitrial_status");

                entity.Property(e => e.gender)
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .IsRequired()
                            .HasColumnName("gender");

                entity.HasCheckConstraint("GENDER_CK",
                    "gender IN ('Male', 'Female')");

                entity.HasOne(e => e.Role).WithMany(d => d.Users)
                .HasForeignKey(e => e.role_id)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("ROLEID_FK");

                entity.Property(e => e.create_date)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("(sysdatetime())")
                    .HasColumnName("create_date");

                entity.Property(e => e.date_of_birth)
                      .HasColumnType("datetime2(0)")
                      .IsRequired()
                      .HasColumnName("date_of_birth");

                entity.HasCheckConstraint(
                      "DATEOFBIRTH_CK",
                      "[date_of_birth] <= GETDATE()"
                );

                entity.Property(e => e.profile_image)
                    .HasColumnType("VARBINARY(MAX)")
                    .HasColumnName("profile_image");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(e => e.role_id).HasName("ROLEID_PK");
                entity.Property(e => e.role_id)
                   .ValueGeneratedOnAdd();

                entity.Property(e => e.role_name)
                   .HasMaxLength(50)
                   .IsUnicode(false)
                   .IsRequired()
                   .HasColumnName("role_name");

                entity.HasIndex(e => e.role_name, "ROLENAME_UQ").IsUnique();

                entity.Property(e => e.create_date)
                   .HasColumnType("datetime2(0)")
                   .HasDefaultValueSql("(sysdatetime())")
                   .HasColumnName("create_date");
            });

            modelBuilder.Entity<Nurses>(entity =>
            {
                entity.HasKey(e => e.nurse_id).HasName("NURSEID_PK");
                entity.Property(e => e.nurse_id)
                 .ValueGeneratedOnAdd();

                entity.Property(e => e.bio)
                     .HasMaxLength(500)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("bio");

                entity.HasOne(e => e.Users)
                     .WithOne(d => d.Nurses)
                     .HasForeignKey<Nurses>(e => e.user_id)
                     .HasConstraintName("NURSEUSERID_FK");

                entity.HasIndex(e => e.user_id, "NURSEUSERID_UQ")
                    .IsUnique();
            });

            modelBuilder.Entity<Doctors>(entity =>
            {
                entity.HasKey(e => e.doctor_id)
                .HasName("DOCTORID_PK");

                entity.Property(e => e.doctor_id)
                     .ValueGeneratedOnAdd();

                entity.Property(e => e.bio)
                     .HasMaxLength(500)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("bio");

                entity.Property(e => e.specialization)
                     .HasMaxLength(500)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("specialization");

                entity.Property(e => e.Type)  // Changed from Type to type
                     .HasMaxLength(100)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("type");

                entity.Property(e => e.experience_years)
                     .IsRequired()
                     .HasColumnName("experience_years");

                entity.HasCheckConstraint("YEARSOFEXPERIENCE_CK",
                    "experience_years >= 0 AND experience_years <= 100");

                entity.HasOne(e => e.Users)
                    .WithOne(d => d.Doctors)
                    .HasForeignKey<Doctors>(e => e.user_id)
                    .HasConstraintName("DOCTORUSERID_FK");

                entity.HasIndex(e => e.user_id, "DOCTORUSERID_UQ")
                    .IsUnique();
            });

            modelBuilder.Entity<Notifications>(entity =>
            {
                entity.HasKey(e => e.notification_id).HasName("NOTIFICATIONID_PK");
                entity.Property(e => e.notification_id)
                 .ValueGeneratedOnAdd();

                entity.Property(e => e.message)
                     .HasMaxLength(500)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("message");

                entity.Property(e => e.is_read)
                   .IsRequired()
                   .HasColumnType<bool>("bit")
                   .HasColumnName("is_read");

                entity.Property(e => e.create_date)
                 .HasColumnType("datetime2(0)")
                 .HasDefaultValueSql("(sysdatetime())")
                 .HasColumnName("create_date");

                entity.HasOne(e => e.SendUsers).WithMany(d => d.SendNotifications)
                     .HasForeignKey(e => e.sender_user_id)
                     .IsRequired()
                     .OnDelete(DeleteBehavior.NoAction)
                     .HasConstraintName("FK_User_SenderNotification");

                entity.HasOne(e => e.RecieverUsers).WithMany(d => d.RecieveNotifications)
                     .HasForeignKey(e => e.reciever_user_id)
                     .IsRequired()
                     .OnDelete(DeleteBehavior.NoAction)
                     .HasConstraintName("FK_User_RecieveNotification");

                entity.Property(e => e.read_date)
                 .HasColumnType("datetime2(0)")
                 .HasColumnName("read_date");

                entity.HasCheckConstraint("READSENDDATE_CK",
                     "read_date >= create_date");
            });

            modelBuilder.Entity<Rooms>(entity =>
            {
                entity.HasKey(e => e.room_id).HasName("ROOMID_PK");
                entity.Property(e => e.room_id)
                 .ValueGeneratedOnAdd();

                entity.HasIndex(e => new { e.room_name, e.room_type }, "UQ_ROOM_NAME_TYPE")
                     .IsUnique();

                entity.Property(e => e.room_name)
                     .HasMaxLength(200)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("room_name");

                entity.Property(e => e.room_type)
                     .HasMaxLength(50)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("room_type");

                entity.Property(e => e.is_active)
                   .IsRequired()
                   .HasColumnType<bool>("bit")
                   .HasColumnName("is_active");

                entity.Property(e => e.create_date)
                 .HasColumnType("datetime2(0)")
                 .HasDefaultValueSql("(sysdatetime())")
                 .HasColumnName("create_date");
            });

            modelBuilder.Entity<Logs>(entity =>
            {
                entity.HasKey(e => e.log_id).HasName("LOGID_PK");
                entity.Property(e => e.log_id)
                 .ValueGeneratedOnAdd();

                entity.Property(e => e.action_type)
                     .HasMaxLength(50)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("action_type");

                entity.Property(e => e.log_type)
                     .HasMaxLength(50)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("log_type");

                entity.Property(e => e.Description)
                     .HasMaxLength(int.MaxValue)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("description");

                entity.Property(e => e.log_date)
                 .HasColumnType("datetime2(0)")
                 .HasDefaultValueSql("(sysdatetime())")
                 .HasColumnName("log_date");
            });

            modelBuilder.Entity<Appointments>(entity =>
            {
                entity.HasKey(e => e.appointment_id).HasName("APPOINTMENTID_PK");
                entity.Property(e => e.appointment_id)
                 .ValueGeneratedOnAdd();

                entity.Property(e => e.status)
                     .HasMaxLength(50)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("status");

                entity.Property(e => e.notes)
                     .HasMaxLength(500)
                     .IsUnicode(false)
                     .HasColumnName("notes");

                entity.Property(e => e.medicine)
                     .HasMaxLength(500)
                     .IsUnicode(false)
                     .HasColumnName("medicine");

                entity.Property(e => e.create_date)
                 .HasColumnType("datetime2(0)")
                 .HasDefaultValueSql("(sysdatetime())")
                 .HasColumnName("create_date");

                entity.Property(e => e.appointment_date)
                 .HasColumnType("datetime2(0)")
                 .IsRequired()
                 .HasColumnName("appointment_date");

                entity.HasOne(e => e.Patients).WithMany(d => d.Appointments)
                     .HasForeignKey(e => e.patient_id)
                     .IsRequired()
                     .OnDelete(DeleteBehavior.NoAction)
                     .HasConstraintName("FK_Patient_Appointment");

                entity.HasOne(e => e.Doctors).WithMany(d => d.Appointments)
                     .HasForeignKey(e => e.doctor_id)
                     .IsRequired()
                     .OnDelete(DeleteBehavior.NoAction)
                     .HasConstraintName("FK_Doctor_Appointment");

                entity.HasOne(e => e.Nurses).WithMany(d => d.Appointments)
                     .HasForeignKey(e => e.nurse_id)
                     .IsRequired(false)
                     .OnDelete(DeleteBehavior.NoAction)
                     .HasConstraintName("FK_Nurse_Appointment");

                entity.HasOne(e => e.Rooms).WithMany(d => d.Appointments)
                     .HasForeignKey(e => e.room_id)
                     .IsRequired(false)
                     .OnDelete(DeleteBehavior.NoAction)
                     .HasConstraintName("FK_Room_Appointment");

                entity.HasCheckConstraint("APPOINTMENTDATE_CK",
                     "appointment_date >= GETDATE()");
            });

            modelBuilder.Entity<FeedBacks>(entity =>
            {
                entity.HasKey(e => e.feedback_id).HasName("FEEDBACKID_PK");
                entity.Property(e => e.feedback_id)
                 .ValueGeneratedOnAdd();

                entity.Property(e => e.comment)
                     .HasMaxLength(500)
                     .IsUnicode(false)
                     .IsRequired()
                     .HasColumnName("comment");

                entity.Property(e => e.doctor_reply)
                     .HasMaxLength(500)
                     .IsUnicode(false)
                     .HasColumnName("doctor_reply");

                entity.Property(e => e.feedback_date)
                 .HasColumnType("datetime2(0)")
                 .HasDefaultValueSql("(sysdatetime())")
                 .HasColumnName("feedback_date");

                entity.Property(e => e.reply_date)
                 .HasColumnType("datetime2(0)")
                 .HasColumnName("reply_date");

                entity.Property(e => e.rate)
                 .IsRequired()
                 .HasColumnName("rate");

                entity.HasCheckConstraint("CK_Rate_Range",
                    "rate >= 1 AND rate <= 5");

                entity.HasCheckConstraint("reply_date_CK",
                    "reply_date >= feedback_date");

                entity.Property(e => e.is_favourite)
                 .IsRequired()
                 .HasColumnType<bool>("bit")
                 .HasColumnName("is_favourite");

                entity.HasOne(e => e.Patients).WithMany(d => d.FeedBacks)
                     .HasForeignKey(e => e.patient_id)
                     .IsRequired()
                     .OnDelete(DeleteBehavior.NoAction)
                     .HasConstraintName("FK_Patient_FeedBack");

                entity.HasOne(e => e.Doctors).WithMany(d => d.FeedBacks)
                     .HasForeignKey(e => e.doctor_id)
                     .IsRequired()
                     .OnDelete(DeleteBehavior.NoAction)
                     .HasConstraintName("FK_Doctor_FeedBack");

                entity.HasOne(a => a.Appointments)
                     .WithOne(f => f.FeedBacks)
                     .HasForeignKey<FeedBacks>(f => f.appointment_id)
                     .IsRequired()
                     .OnDelete(DeleteBehavior.NoAction)
                     .HasConstraintName("FK_Appointment_FeedBack");

                // Add unique constraint for appointment_id
                entity.HasIndex(e => e.appointment_id, "APPOINTMENTID_UK")
                    .IsUnique();
            });

            modelBuilder.Entity<Reports>(entity =>
            {
                entity.ToTable("Reports");

                entity.HasKey(e => e.report_id);

                entity.Property(e => e.report_id)
                    .HasColumnName("report_id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.report_file)
                    .IsRequired()
                    .HasColumnName("report_file")
                    .HasColumnType("VARBINARY(MAX)");

                entity.Property(e => e.file_format)
                    .IsRequired()
                    .HasColumnName("file_format")
                    .HasMaxLength(50);

                entity.Property(e => e.ReportDate)  // Changed from ReportDate to report_date
                    .IsRequired()
                    .HasColumnName("report_date")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.report_type)
                    .IsRequired()
                    .HasColumnName("report_type")
                    .HasMaxLength(100);

                entity.Property(e => e.period_type)
                    .HasColumnName("period_type")
                    .HasMaxLength(200);

                entity.Property(e => e.description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.file_name)  // Changed to match SQL column name
                    .IsRequired()
                    .HasColumnName("file_name")
                    .HasMaxLength(255);
            });
        }
    }
}