using medibook_API.Extensions.DTOs;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Linq;

namespace medibook_API.Extensions.Services
{
    public class EmailServices
    {
        private readonly EmailSettingsDto _emailSettings;
        private readonly ILogger<EmailServices> _logger;

        public EmailServices(IOptions<EmailSettingsDto> emailSettings, ILogger<EmailServices> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendVerificationEmailAsync(string recipientEmail,string firstname , string lastname,string gender, string verificationCode)
        {


            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning("Recipient email is null or empty");
                return false;
            }

            try
            {
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    client.EnableSsl = _emailSettings.EnableSsl;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);



                    var from = new MailAddress(_emailSettings.Email, "MediBook System");
                    var to = new MailAddress(recipientEmail);

                    string emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{
            font-family: 'Segoe UI', Arial, sans-serif;
            background: #eef3f7;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            background: #ffffff;
            margin: 40px auto;
            border-radius: 14px;
            box-shadow: 0 8px 25px rgba(0,0,0,0.08);
            overflow: hidden;
            border-top: 6px solid #2bb3c0;
        }}
        .header {{
            background: #2bb3c0;
            padding: 30px;
            text-align: center;
            color: #ffffff;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
            font-weight: 600;
        }}
        .content {{
            padding: 30px;
            text-align: center;
            color: #333;
        }}
        .content p {{
            font-size: 16px;
            margin-bottom: 15px;
        }}
        .code-box {{
            display: inline-block;
            background: #e8fcff;
            border: 2px solid #2bb3c0;
            padding: 20px 30px;
            border-radius: 10px;
            font-size: 40px;
            font-weight: bold;
            color: #056571;
            letter-spacing: 6px;
            margin: 20px 0;
        }}
        .footer {{
            background: #f7f7f7;
            padding: 18px;
            text-align: center;
            font-size: 13px;
            color: #777;
        }}
        .logo {{
            width: 80px;
            margin-bottom: 15px;
        }}
    </style>
</head>

<body>
<div class='container'>

    <div class='header'>
        <h1>MediBook Verification</h1>
    </div>

    <div class='content'>
        <p>Dear {(gender.ToLower() == "male" ? "Mr" : "Mrs")},{firstname} {lastname}</p>
        <p>To complete your verification with the <strong>MediBook System</strong>, please use the code below:</p>

        <div class='code-box'>{verificationCode}</div>

        <p>This code is valid for a limited time.  
        <br>If you did not request this code, please ignore this message.</p>
    </div>

    <div class='footer'>
        © {DateTime.Now.Year} MediBook Team — Medical Appointment & Patient Management System
    </div>

</div>
</body>
</html>";

                    var message = new MailMessage(from, to)
                    {
                        Subject = "MediBook Verification Code",
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        Body = emailBody,
                        BodyEncoding = System.Text.Encoding.UTF8,
                        IsBodyHtml = true
                    };

                    await client.SendMailAsync(message);

                    _logger.LogInformation($"Verification email sent successfully to {recipientEmail}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification email");
                return false;
            }
        }

        // 1. Send email when appointment is created (to doctor and patient)
        public async Task<bool> SendAppointmentCreatedEmailAsync(
            string doctorEmail, string doctorName,
            string patientEmail, string patientName,
            AppointmentDetailsDto appointmentDetails)
        {
            try
            {
                var tasks = new List<Task<bool>>();

                if (!string.IsNullOrEmpty(doctorEmail))
                    tasks.Add(SendAppointmentCreatedEmailToDoctorAsync(doctorEmail, doctorName, appointmentDetails));

                if (!string.IsNullOrEmpty(patientEmail))
                    tasks.Add(SendAppointmentCreatedEmailToPatientAsync(patientEmail, patientName, appointmentDetails));

                var results = await Task.WhenAll(tasks);
                return results.All(r => r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send appointment created emails");
                return false;
            }
        }

        // 2. Send email when appointment is assigned (to nurse, doctor, and patient)
        public async Task<bool> SendAppointmentAssignedEmailAsync(
            string nurseEmail, string nurseName,
            string doctorEmail, string doctorName,
            string patientEmail, string patientName,
            AppointmentDetailsDto appointmentDetails)
        {
            try
            {
                var tasks = new List<Task<bool>>();

                if (!string.IsNullOrEmpty(nurseEmail))
                    tasks.Add(SendAppointmentAssignedEmailToNurseAsync(nurseEmail, nurseName, appointmentDetails));

                if (!string.IsNullOrEmpty(doctorEmail))
                    tasks.Add(SendAppointmentAssignedEmailToDoctorAsync(doctorEmail, doctorName, appointmentDetails));

                if (!string.IsNullOrEmpty(patientEmail))
                    tasks.Add(SendAppointmentAssignedEmailToPatientAsync(patientEmail, patientName, appointmentDetails));

                var results = await Task.WhenAll(tasks);
                return results.All(r => r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send appointment assigned emails");
                return false;
            }
        }

        // 3. Send email when appointment is cancelled (to patient and doctor)
        public async Task<bool> SendAppointmentCancelledEmailAsync(
            string doctorEmail, string doctorName,
            string patientEmail, string patientName,
            AppointmentDetailsDto appointmentDetails,
            string cancellationReason)
        {
            try
            {
                var tasks = new List<Task<bool>>();

                if (!string.IsNullOrEmpty(doctorEmail))
                    tasks.Add(SendAppointmentCancelledEmailToDoctorAsync(doctorEmail, doctorName, appointmentDetails, cancellationReason));

                if (!string.IsNullOrEmpty(patientEmail))
                    tasks.Add(SendAppointmentCancelledEmailToPatientAsync(patientEmail, patientName, appointmentDetails, cancellationReason));

                var results = await Task.WhenAll(tasks);
                return results.All(r => r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send appointment cancelled emails");
                return false;
            }
        }

        // 4. Send email when appointment is closed (to nurse, doctor, and patient)
        public async Task<bool> SendAppointmentClosedEmailAsync(
            string nurseEmail, string nurseName,
            string doctorEmail, string doctorName,
            string patientEmail, string patientName,
            AppointmentDetailsDto appointmentDetails,
            string notes, string medicine)
        {
            try
            {
                var tasks = new List<Task<bool>>();

                if (!string.IsNullOrEmpty(nurseEmail))
                    tasks.Add(SendAppointmentClosedEmailToNurseAsync(nurseEmail, nurseName, appointmentDetails, notes, medicine));

                if (!string.IsNullOrEmpty(doctorEmail))
                    tasks.Add(SendAppointmentClosedEmailToDoctorAsync(doctorEmail, doctorName, appointmentDetails, notes, medicine));

                if (!string.IsNullOrEmpty(patientEmail))
                    tasks.Add(SendAppointmentClosedEmailToPatientAsync(patientEmail, patientName, appointmentDetails, notes, medicine));

                var results = await Task.WhenAll(tasks);
                return results.All(r => r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send appointment closed emails");
                return false;
            }
        }

        // 5. Send email when feedback is created (to doctor and patient)
        public async Task<bool> SendFeedbackCreatedEmailAsync(
            string doctorEmail, string doctorName,
            string patientEmail, string patientName,
            int feedbackId, int rate, string comment, int appointmentId)
        {
            try
            {
                var tasks = new List<Task<bool>>();

                if (!string.IsNullOrEmpty(doctorEmail))
                    tasks.Add(SendFeedbackCreatedEmailToDoctorAsync(doctorEmail, doctorName, feedbackId, rate, comment, appointmentId));

                if (!string.IsNullOrEmpty(patientEmail))
                    tasks.Add(SendFeedbackCreatedEmailToPatientAsync(patientEmail, patientName, feedbackId, rate, comment, appointmentId));

                var results = await Task.WhenAll(tasks);
                return results.All(r => r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send feedback created emails");
                return false;
            }
        }

        // 6. Send email when doctor replies to feedback (to patient)
        public async Task<bool> SendDoctorReplyEmailAsync(
            string patientEmail, string patientName,
            string doctorName,
            int feedbackId, string doctorReply, int appointmentId)
        {
            try
            {
                if (string.IsNullOrEmpty(patientEmail))
                {
                    _logger.LogWarning("Patient email is null or empty");
                    return false;
                }

                return await SendDoctorReplyEmailToPatientAsync(patientEmail, patientName, doctorName, feedbackId, doctorReply, appointmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send doctor reply email");
                return false;
            }
        }

        #region Private Email Helper Methods

        private async Task<bool> SendAppointmentCreatedEmailToDoctorAsync(string email, string name, AppointmentDetailsDto details)
        {
            return await SendEmailAsync(email, "New Appointment Scheduled - MediBook",
                GetAppointmentCreatedEmailBody(name, details, "Doctor", true));
        }

        private async Task<bool> SendAppointmentCreatedEmailToPatientAsync(string email, string name, AppointmentDetailsDto details)
        {
            return await SendEmailAsync(email, "Appointment Confirmed - MediBook",
                GetAppointmentCreatedEmailBody(name, details, "Patient", false));
        }

        private async Task<bool> SendAppointmentAssignedEmailToNurseAsync(string email, string name, AppointmentDetailsDto details)
        {
            return await SendEmailAsync(email, "Appointment Assigned to You - MediBook",
                GetAppointmentAssignedEmailBody(name, details, "Nurse"));
        }

        private async Task<bool> SendAppointmentAssignedEmailToDoctorAsync(string email, string name, AppointmentDetailsDto details)
        {
            return await SendEmailAsync(email, "Appointment Assigned - MediBook",
                GetAppointmentAssignedEmailBody(name, details, "Doctor"));
        }

        private async Task<bool> SendAppointmentAssignedEmailToPatientAsync(string email, string name, AppointmentDetailsDto details)
        {
            return await SendEmailAsync(email, "Appointment Assigned - MediBook",
                GetAppointmentAssignedEmailBody(name, details, "Patient"));
        }

        private async Task<bool> SendAppointmentCancelledEmailToDoctorAsync(string email, string name, AppointmentDetailsDto details, string reason)
        {
            return await SendEmailAsync(email, "Appointment Cancelled - MediBook",
                GetAppointmentCancelledEmailBody(name, details, reason, "Doctor"));
        }

        private async Task<bool> SendAppointmentCancelledEmailToPatientAsync(string email, string name, AppointmentDetailsDto details, string reason)
        {
            return await SendEmailAsync(email, "Appointment Cancelled - MediBook",
                GetAppointmentCancelledEmailBody(name, details, reason, "Patient"));
        }

        private async Task<bool> SendAppointmentClosedEmailToNurseAsync(string email, string name, AppointmentDetailsDto details, string notes, string medicine)
        {
            return await SendEmailAsync(email, "Appointment Completed - MediBook",
                GetAppointmentClosedEmailBody(name, details, notes, medicine, "Nurse"));
        }

        private async Task<bool> SendAppointmentClosedEmailToDoctorAsync(string email, string name, AppointmentDetailsDto details, string notes, string medicine)
        {
            return await SendEmailAsync(email, "Appointment Completed - MediBook",
                GetAppointmentClosedEmailBody(name, details, notes, medicine, "Doctor"));
        }

        private async Task<bool> SendAppointmentClosedEmailToPatientAsync(string email, string name, AppointmentDetailsDto details, string notes, string medicine)
        {
            return await SendEmailAsync(email, "Appointment Completed - MediBook",
                GetAppointmentClosedEmailBody(name, details, notes, medicine, "Patient"));
        }

        private async Task<bool> SendFeedbackCreatedEmailToDoctorAsync(string email, string name, int feedbackId, int rate, string comment, int appointmentId)
        {
            return await SendEmailAsync(email, "New Feedback Received - MediBook",
                GetFeedbackCreatedEmailBody(name, feedbackId, rate, comment, appointmentId, "Doctor"));
        }

        private async Task<bool> SendFeedbackCreatedEmailToPatientAsync(string email, string name, int feedbackId, int rate, string comment, int appointmentId)
        {
            return await SendEmailAsync(email, "Feedback Submitted - MediBook",
                GetFeedbackCreatedEmailBody(name, feedbackId, rate, comment, appointmentId, "Patient"));
        }

        private async Task<bool> SendDoctorReplyEmailToPatientAsync(string email, string name, string doctorName, int feedbackId, string doctorReply, int appointmentId)
        {
            return await SendEmailAsync(email, "Doctor's Reply to Your Feedback - MediBook",
                GetDoctorReplyEmailBody(name, doctorName, feedbackId, doctorReply, appointmentId));
        }

        private async Task<bool> SendEmailAsync(string recipientEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning("Recipient email is null or empty");
                return false;
            }

            try
            {
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    client.EnableSsl = _emailSettings.EnableSsl;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);

                    var from = new MailAddress(_emailSettings.Email, "MediBook System");
                    var to = new MailAddress(recipientEmail);

                    var message = new MailMessage(from, to)
                    {
                        Subject = subject,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        Body = htmlBody,
                        BodyEncoding = System.Text.Encoding.UTF8,
                        IsBodyHtml = true
                    };

                    await client.SendMailAsync(message);
                    _logger.LogInformation($"Email sent successfully to {recipientEmail}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {recipientEmail}");
                return false;
            }
        }

        #endregion

        #region Email Body Templates

        private string GetAppointmentCreatedEmailBody(string recipientName, AppointmentDetailsDto details, string recipientType, bool isDoctor)
        {
            var appointmentDate = details.AppointmentDate.ToString("dddd, MMMM dd, yyyy 'at' hh:mm tt");
            var greeting = isDoctor ? "Dr." : "";
            
            return $@"<!DOCTYPE html><html><head><meta charset='UTF-8'><style>body{{font-family:'Segoe UI',Arial,sans-serif;background:#eef3f7;margin:0;padding:0}}.container{{max-width:600px;background:#ffffff;margin:40px auto;border-radius:14px;box-shadow:0 8px 25px rgba(0,0,0,0.08);overflow:hidden;border-top:6px solid #2bb3c0}}.header{{background:#2bb3c0;padding:30px;text-align:center;color:#ffffff}}.header h1{{margin:0;font-size:28px;font-weight:600}}.content{{padding:30px;color:#333}}.content p{{font-size:16px;margin-bottom:15px;line-height:1.6}}.details-box{{background:#f8f9fa;border-left:4px solid #2bb3c0;padding:20px;margin:20px 0;border-radius:8px}}.details-box strong{{color:#056571}}.footer{{background:#f7f7f7;padding:18px;text-align:center;font-size:13px;color:#777}}</style></head><body><div class='container'><div class='header'><h1>📅 New Appointment Scheduled</h1></div><div class='content'><p>Dear {greeting} {recipientName},</p><p>A new appointment has been scheduled with the following details:</p><div class='details-box'><p><strong>Appointment ID:</strong> {details.AppointmentId}</p><p><strong>Date & Time:</strong> {appointmentDate}</p><p><strong>Doctor:</strong> Dr. {details.DoctorFirstName} {details.DoctorLastName} ({details.DoctorSpecialization})</p><p><strong>Patient:</strong> {details.PatientFirstName} {details.PatientLastName}</p><p><strong>Status:</strong> {details.Status}</p></div><p>Please make sure to be available at the scheduled time.</p><p>If you have any questions or need to reschedule, please contact us.</p></div><div class='footer'>© {DateTime.Now.Year} MediBook Team — Medical Appointment & Patient Management System</div></div></body></html>";
        }

        private string GetAppointmentAssignedEmailBody(string recipientName, AppointmentDetailsDto details, string recipientType)
        {
            var appointmentDate = details.AppointmentDate.ToString("dddd, MMMM dd, yyyy 'at' hh:mm tt");
            return $@"<!DOCTYPE html><html><head><meta charset='UTF-8'><style>body{{font-family:'Segoe UI',Arial,sans-serif;background:#eef3f7;margin:0;padding:0}}.container{{max-width:600px;background:#ffffff;margin:40px auto;border-radius:14px;box-shadow:0 8px 25px rgba(0,0,0,0.08);overflow:hidden;border-top:6px solid #28a745}}.header{{background:#28a745;padding:30px;text-align:center;color:#ffffff}}.header h1{{margin:0;font-size:28px;font-weight:600}}.content{{padding:30px;color:#333}}.content p{{font-size:16px;margin-bottom:15px;line-height:1.6}}.details-box{{background:#f8f9fa;border-left:4px solid #28a745;padding:20px;margin:20px 0;border-radius:8px}}.details-box strong{{color:#155724}}.footer{{background:#f7f7f7;padding:18px;text-align:center;font-size:13px;color:#777}}</style></head><body><div class='container'><div class='header'><h1>✅ Appointment Assigned</h1></div><div class='content'><p>Dear {recipientName},</p><p>An appointment has been assigned with the following details:</p><div class='details-box'><p><strong>Appointment ID:</strong> {details.AppointmentId}</p><p><strong>Date & Time:</strong> {appointmentDate}</p><p><strong>Doctor:</strong> Dr. {details.DoctorFirstName} {details.DoctorLastName}</p><p><strong>Patient:</strong> {details.PatientFirstName} {details.PatientLastName}</p><p><strong>Nurse:</strong> {details.NurseFirstName} {details.NurseLastName}</p><p><strong>Room:</strong> {details.RoomName} ({details.RoomType})</p><p><strong>Status:</strong> {details.Status}</p></div><p>Please prepare for the appointment accordingly.</p></div><div class='footer'>© {DateTime.Now.Year} MediBook Team — Medical Appointment & Patient Management System</div></div></body></html>";
        }

        private string GetAppointmentCancelledEmailBody(string recipientName, AppointmentDetailsDto details, string reason, string recipientType)
        {
            var appointmentDate = details.AppointmentDate.ToString("dddd, MMMM dd, yyyy 'at' hh:mm tt");
            return $@"<!DOCTYPE html><html><head><meta charset='UTF-8'><style>body{{font-family:'Segoe UI',Arial,sans-serif;background:#eef3f7;margin:0;padding:0}}.container{{max-width:600px;background:#ffffff;margin:40px auto;border-radius:14px;box-shadow:0 8px 25px rgba(0,0,0,0.08);overflow:hidden;border-top:6px solid #dc3545}}.header{{background:#dc3545;padding:30px;text-align:center;color:#ffffff}}.header h1{{margin:0;font-size:28px;font-weight:600}}.content{{padding:30px;color:#333}}.content p{{font-size:16px;margin-bottom:15px;line-height:1.6}}.details-box{{background:#fff5f5;border-left:4px solid #dc3545;padding:20px;margin:20px 0;border-radius:8px}}.details-box strong{{color:#721c24}}.footer{{background:#f7f7f7;padding:18px;text-align:center;font-size:13px;color:#777}}</style></head><body><div class='container'><div class='header'><h1>❌ Appointment Cancelled</h1></div><div class='content'><p>Dear {recipientName},</p><p>We regret to inform you that the following appointment has been cancelled:</p><div class='details-box'><p><strong>Appointment ID:</strong> {details.AppointmentId}</p><p><strong>Date & Time:</strong> {appointmentDate}</p><p><strong>Doctor:</strong> Dr. {details.DoctorFirstName} {details.DoctorLastName}</p><p><strong>Patient:</strong> {details.PatientFirstName} {details.PatientLastName}</p><p><strong>Cancellation Reason:</strong> {reason}</p></div><p>If you need to reschedule, please contact us at your earliest convenience.</p></div><div class='footer'>© {DateTime.Now.Year} MediBook Team — Medical Appointment & Patient Management System</div></div></body></html>";
        }

        private string GetAppointmentClosedEmailBody(string recipientName, AppointmentDetailsDto details, string notes, string medicine, string recipientType)
        {
            var appointmentDate = details.AppointmentDate.ToString("dddd, MMMM dd, yyyy 'at' hh:mm tt");
            return $@"<!DOCTYPE html><html><head><meta charset='UTF-8'><style>body{{font-family:'Segoe UI',Arial,sans-serif;background:#eef3f7;margin:0;padding:0}}.container{{max-width:600px;background:#ffffff;margin:40px auto;border-radius:14px;box-shadow:0 8px 25px rgba(0,0,0,0.08);overflow:hidden;border-top:6px solid #17a2b8}}.header{{background:#17a2b8;padding:30px;text-align:center;color:#ffffff}}.header h1{{margin:0;font-size:28px;font-weight:600}}.content{{padding:30px;color:#333}}.content p{{font-size:16px;margin-bottom:15px;line-height:1.6}}.details-box{{background:#f8f9fa;border-left:4px solid #17a2b8;padding:20px;margin:20px 0;border-radius:8px}}.details-box strong{{color:#0c5460}}.footer{{background:#f7f7f7;padding:18px;text-align:center;font-size:13px;color:#777}}</style></head><body><div class='container'><div class='header'><h1>✅ Appointment Completed</h1></div><div class='content'><p>Dear {recipientName},</p><p>The following appointment has been completed:</p><div class='details-box'><p><strong>Appointment ID:</strong> {details.AppointmentId}</p><p><strong>Date & Time:</strong> {appointmentDate}</p><p><strong>Doctor:</strong> Dr. {details.DoctorFirstName} {details.DoctorLastName}</p><p><strong>Patient:</strong> {details.PatientFirstName} {details.PatientLastName}</p><p><strong>Nurse:</strong> {details.NurseFirstName} {details.NurseLastName}</p><p><strong>Room:</strong> {details.RoomName}</p><p><strong>Medicine Prescribed:</strong> {(string.IsNullOrEmpty(medicine) ? "None" : medicine)}</p><p><strong>Notes:</strong> {(string.IsNullOrEmpty(notes) ? "None" : notes)}</p></div><p>Thank you for using MediBook services.</p></div><div class='footer'>© {DateTime.Now.Year} MediBook Team — Medical Appointment & Patient Management System</div></div></body></html>";
        }

        private string GetFeedbackCreatedEmailBody(string recipientName, int feedbackId, int rate, string comment, int appointmentId, string recipientType)
        {
            var isDoctor = recipientType == "Doctor";
            var greeting = isDoctor ? "You have received new feedback" : "Your feedback has been submitted";
            var stars = new string('⭐', rate) + new string('☆', 5 - rate);
            return $@"<!DOCTYPE html><html><head><meta charset='UTF-8'><style>body{{font-family:'Segoe UI',Arial,sans-serif;background:#eef3f7;margin:0;padding:0}}.container{{max-width:600px;background:#ffffff;margin:40px auto;border-radius:14px;box-shadow:0 8px 25px rgba(0,0,0,0.08);overflow:hidden;border-top:6px solid #ffc107}}.header{{background:#ffc107;padding:30px;text-align:center;color:#333}}.header h1{{margin:0;font-size:28px;font-weight:600}}.content{{padding:30px;color:#333}}.content p{{font-size:16px;margin-bottom:15px;line-height:1.6}}.details-box{{background:#fffbf0;border-left:4px solid #ffc107;padding:20px;margin:20px 0;border-radius:8px}}.details-box strong{{color:#856404}}.rating{{font-size:24px;color:#ffc107}}.footer{{background:#f7f7f7;padding:18px;text-align:center;font-size:13px;color:#777}}</style></head><body><div class='container'><div class='header'><h1>💬 New Feedback</h1></div><div class='content'><p>Dear {recipientName},</p><p>{greeting} for appointment #{appointmentId}:</p><div class='details-box'><p><strong>Feedback ID:</strong> {feedbackId}</p><p><strong>Appointment ID:</strong> {appointmentId}</p><p><strong>Rating:</strong> <span class='rating'>{stars}</span> ({rate}/5)</p><p><strong>Comment:</strong> {comment}</p></div><p>Thank you for your valuable feedback.</p></div><div class='footer'>© {DateTime.Now.Year} MediBook Team — Medical Appointment & Patient Management System</div></div></body></html>";
        }

        private string GetDoctorReplyEmailBody(string patientName, string doctorName, int feedbackId, string doctorReply, int appointmentId)
        {
            return $@"<!DOCTYPE html><html><head><meta charset='UTF-8'><style>body{{font-family:'Segoe UI',Arial,sans-serif;background:#eef3f7;margin:0;padding:0}}.container{{max-width:600px;background:#ffffff;margin:40px auto;border-radius:14px;box-shadow:0 8px 25px rgba(0,0,0,0.08);overflow:hidden;border-top:6px solid #6f42c1}}.header{{background:#6f42c1;padding:30px;text-align:center;color:#ffffff}}.header h1{{margin:0;font-size:28px;font-weight:600}}.content{{padding:30px;color:#333}}.content p{{font-size:16px;margin-bottom:15px;line-height:1.6}}.details-box{{background:#f3f0ff;border-left:4px solid #6f42c1;padding:20px;margin:20px 0;border-radius:8px}}.details-box strong{{color:#4a2c5a}}.reply-box{{background:#ffffff;border:2px solid #6f42c1;padding:15px;margin:15px 0;border-radius:8px;font-style:italic}}.footer{{background:#f7f7f7;padding:18px;text-align:center;font-size:13px;color:#777}}</style></head><body><div class='container'><div class='header'><h1>💬 Doctor's Reply</h1></div><div class='content'><p>Dear {patientName},</p><p>Dr. {doctorName} has replied to your feedback:</p><div class='details-box'><p><strong>Feedback ID:</strong> {feedbackId}</p><p><strong>Appointment ID:</strong> {appointmentId}</p></div><div class='reply-box'><p><strong>Doctor's Reply:</strong></p><p>{doctorReply}</p></div><p>Thank you for your continued trust in our services.</p></div><div class='footer'>© {DateTime.Now.Year} MediBook Team — Medical Appointment & Patient Management System</div></div></body></html>";
        }

        #endregion
    }
}
