using medibook_API.Extensions.DTOs;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

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

        public async Task<bool> SendVerificationEmailAsync(string recipientEmail, string firstname, string lastname, string gender, string verificationCode)
        {
            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning("Recipient email is null or empty");
                return false;
            }

            try
            {
                using (var client = CreateSmtpClient())
                {
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
        .appointment-details {{
            background: #f9f9f9;
            border-left: 4px solid #2bb3c0;
            padding: 15px;
            margin: 20px 0;
            text-align: left;
            border-radius: 4px;
        }}
        .appointment-details p {{
            margin: 8px 0;
        }}
        .button {{
            display: inline-block;
            background: #2bb3c0;
            color: white;
            padding: 12px 24px;
            text-decoration: none;
            border-radius: 6px;
            margin: 10px 0;
            font-weight: bold;
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

        // Send email when appointment is booked
        public async Task<bool> SendAppointmentBookedEmailAsync(
            string patientEmail,
            string doctorEmail,
            string patientName,
            string doctorName,
            DateTime appointmentDate,
            TimeSpan appointmentTime,
            string appointmentId,
            string location = "Main Hospital")
        {
            var tasks = new List<Task<bool>>();

            // Send to patient
            if (!string.IsNullOrEmpty(patientEmail))
            {
                tasks.Add(SendAppointmentBookedPatientEmail(patientEmail, patientName, doctorName, appointmentDate, appointmentTime, appointmentId, location));
            }

            // Send to doctor
            if (!string.IsNullOrEmpty(doctorEmail))
            {
                tasks.Add(SendAppointmentBookedDoctorEmail(doctorEmail, doctorName, patientName, appointmentDate, appointmentTime, appointmentId, location));
            }

            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }

        // Send email when appointment is closed/completed
        public async Task<bool> SendAppointmentClosedEmailAsync(
            string patientEmail,
            string doctorEmail,
            string patientName,
            string doctorName,
            DateTime appointmentDate,
            string appointmentId,
            string notes = null)
        {
            var tasks = new List<Task<bool>>();

            // Send to patient
            if (!string.IsNullOrEmpty(patientEmail))
            {
                tasks.Add(SendAppointmentClosedPatientEmail(patientEmail, patientName, doctorName, appointmentDate, appointmentId, notes));
            }

            // Send to doctor
            if (!string.IsNullOrEmpty(doctorEmail))
            {
                tasks.Add(SendAppointmentClosedDoctorEmail(doctorEmail, doctorName, patientName, appointmentDate, appointmentId, notes));
            }

            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }

        // Send email when nurse is assigned to appointment
        public async Task<bool> SendNurseAssignedEmailAsync(
            string patientEmail,
            string doctorEmail,
            string nurseEmail,
            string patientName,
            string doctorName,
            string nurseName,
            DateTime appointmentDate,
            TimeSpan appointmentTime,
            string appointmentId)
        {
            var tasks = new List<Task<bool>>();

            // Send to patient
            if (!string.IsNullOrEmpty(patientEmail))
            {
                tasks.Add(SendNurseAssignedPatientEmail(patientEmail, patientName, doctorName, nurseName, appointmentDate, appointmentTime, appointmentId));
            }

            // Send to doctor
            if (!string.IsNullOrEmpty(doctorEmail))
            {
                tasks.Add(SendNurseAssignedDoctorEmail(doctorEmail, doctorName, nurseName, patientName, appointmentDate, appointmentTime, appointmentId));
            }

            // Send to nurse
            if (!string.IsNullOrEmpty(nurseEmail))
            {
                tasks.Add(SendNurseAssignedNurseEmail(nurseEmail, nurseName, doctorName, patientName, appointmentDate, appointmentTime, appointmentId));
            }

            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }

        // Send email when appointment is closed with nurse involved
        public async Task<bool> SendAppointmentClosedWithNurseEmailAsync(
            string patientEmail,
            string doctorEmail,
            string nurseEmail,
            string patientName,
            string doctorName,
            string nurseName,
            DateTime appointmentDate,
            string appointmentId,
            string notes = null)
        {
            var tasks = new List<Task<bool>>();

            // Send to patient
            if (!string.IsNullOrEmpty(patientEmail))
            {
                tasks.Add(SendAppointmentClosedWithNursePatientEmail(patientEmail, patientName, doctorName, nurseName, appointmentDate, appointmentId, notes));
            }

            // Send to doctor
            if (!string.IsNullOrEmpty(doctorEmail))
            {
                tasks.Add(SendAppointmentClosedWithNurseDoctorEmail(doctorEmail, doctorName, patientName, nurseName, appointmentDate, appointmentId, notes));
            }

            // Send to nurse
            if (!string.IsNullOrEmpty(nurseEmail))
            {
                tasks.Add(SendAppointmentClosedWithNurseNurseEmail(nurseEmail, nurseName, doctorName, patientName, appointmentDate, appointmentId, notes));
            }

            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }

        // Send email when feedback is provided
        public async Task<bool> SendFeedbackEmailAsync(
            string patientEmail,
            string doctorEmail,
            string patientName,
            string doctorName,
            DateTime appointmentDate,
            string appointmentId,
            string feedback,
            int rating)
        {
            var tasks = new List<Task<bool>>();

            // Send to patient (acknowledgement)
            if (!string.IsNullOrEmpty(patientEmail))
            {
                tasks.Add(SendFeedbackPatientEmail(patientEmail, patientName, doctorName, appointmentDate, appointmentId, feedback, rating));
            }

            // Send to doctor (notification of feedback)
            if (!string.IsNullOrEmpty(doctorEmail))
            {
                tasks.Add(SendFeedbackDoctorEmail(doctorEmail, doctorName, patientName, appointmentDate, appointmentId, feedback, rating));
            }

            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }

        // Send email when doctor replies to feedback
        public async Task<bool> SendDoctorReplyEmailAsync(
            string patientEmail,
            string doctorEmail,
            string patientName,
            string doctorName,
            DateTime appointmentDate,
            string appointmentId,
            string replyMessage)
        {
            var tasks = new List<Task<bool>>();

            // Send to patient (doctor's reply)
            if (!string.IsNullOrEmpty(patientEmail))
            {
                tasks.Add(SendDoctorReplyPatientEmail(patientEmail, patientName, doctorName, appointmentDate, appointmentId, replyMessage));
            }

            // Send to doctor (confirmation of reply sent)
            if (!string.IsNullOrEmpty(doctorEmail))
            {
                tasks.Add(SendDoctorReplyDoctorEmail(doctorEmail, doctorName, patientName, appointmentDate, appointmentId, replyMessage));
            }

            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }


        private SmtpClient CreateSmtpClient()
        {
            var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                EnableSsl = _emailSettings.EnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password)
            };
            return client;
        }

        private async Task<bool> SendEmailAsync(string recipientEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning("Recipient email is null or empty");
                return false;
            }

            try
            {
                using (var client = CreateSmtpClient())
                {
                    var from = new MailAddress(_emailSettings.Email, "MediBook System");
                    var to = new MailAddress(recipientEmail);

                    var message = new MailMessage(from, to)
                    {
                        Subject = subject,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        Body = body,
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

        private string GetEmailTemplate(string header, string salutation, string mainContent, string additionalInfo = "")
        {
            return $@"
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
            text-align: left;
        }}
        .appointment-details {{
            background: #f9f9f9;
            border-left: 4px solid #2bb3c0;
            padding: 15px;
            margin: 20px 0;
            text-align: left;
            border-radius: 4px;
        }}
        .appointment-details p {{
            margin: 8px 0;
        }}
        .footer {{
            background: #f7f7f7;
            padding: 18px;
            text-align: center;
            font-size: 13px;
            color: #777;
        }}
        .button {{
            display: inline-block;
            background: #2bb3c0;
            color: white;
            padding: 12px 24px;
            text-decoration: none;
            border-radius: 6px;
            margin: 10px 0;
            font-weight: bold;
        }}
    </style>
</head>

<body>
<div class='container'>

    <div class='header'>
        <h1>{header}</h1>
    </div>

    <div class='content'>
        <p>{salutation}</p>
        
        {mainContent}

        {additionalInfo}
    </div>

    <div class='footer'>
        © {DateTime.Now.Year} MediBook Team — Medical Appointment & Patient Management System
    </div>

</div>
</body>
</html>";
        }



        private async Task<bool> SendAppointmentBookedPatientEmail(string email, string patientName, string doctorName, DateTime date, TimeSpan time, string appointmentId, string location)
        {
            string header = "Appointment Confirmed";
            string salutation = $"Dear {patientName},";
            string mainContent = $@"
        <p>Your appointment has been successfully booked with <strong>Dr. {doctorName}</strong>.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Doctor:</strong> Dr. {doctorName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
            <p><strong>Time:</strong> {time:hh\\:mm tt}</p>
            <p><strong>Location:</strong> {location}</p>
        </div>

        <p>Please arrive 15 minutes before your scheduled appointment time.</p>
        <p>If you need to reschedule or cancel, please do so at least 24 hours in advance.</p>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "Appointment Confirmation - MediBook", body);
        }

        private async Task<bool> SendAppointmentBookedDoctorEmail(string email, string doctorName, string patientName, DateTime date, TimeSpan time, string appointmentId, string location)
        {
            string header = "New Appointment Scheduled";
            string salutation = $"Dear Dr. {doctorName},";
            string mainContent = $@"
        <p>A new appointment has been scheduled with you.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Patient:</strong> {patientName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
            <p><strong>Time:</strong> {time:hh\\:mm tt}</p>
            <p><strong>Location:</strong> {location}</p>
        </div>

        <p>Please review the patient's medical history before the appointment.</p>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "New Appointment Notification - MediBook", body);
        }

        private async Task<bool> SendAppointmentClosedPatientEmail(string email, string patientName, string doctorName, DateTime date, string appointmentId, string notes)
        {
            string header = "Appointment Completed";
            string salutation = $"Dear {patientName},";
            string mainContent = $@"
        <p>Your appointment with <strong>Dr. {doctorName}</strong> on {date:dddd, MMMM dd, yyyy} has been completed.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Doctor:</strong> Dr. {doctorName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
        </div>";

            string additionalInfo = string.IsNullOrEmpty(notes) ? "" : $@"
        <p><strong>Doctor's Notes:</strong></p>
        <div class='appointment-details'>
            <p>{notes}</p>
        </div>";

            string body = GetEmailTemplate(header, salutation, mainContent, additionalInfo);
            return await SendEmailAsync(email, "Appointment Completed - MediBook", body);
        }

        private async Task<bool> SendAppointmentClosedDoctorEmail(string email, string doctorName, string patientName, DateTime date, string appointmentId, string notes)
        {
            string header = "Appointment Marked as Completed";
            string salutation = $"Dear Dr. {doctorName},";
            string mainContent = $@"
        <p>Your appointment with <strong>{patientName}</strong> on {date:dddd, MMMM dd, yyyy} has been marked as completed.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Patient:</strong> {patientName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
        </div>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "Appointment Completed - MediBook", body);
        }

        private async Task<bool> SendNurseAssignedPatientEmail(string email, string patientName, string doctorName, string nurseName, DateTime date, TimeSpan time, string appointmentId)
        {
            string header = "Nurse Assigned to Your Appointment";
            string salutation = $"Dear {patientName},";
            string mainContent = $@"
        <p>A nurse has been assigned to assist with your upcoming appointment.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Doctor:</strong> Dr. {doctorName}</p>
            <p><strong>Assigned Nurse:</strong> {nurseName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
            <p><strong>Time:</strong> {time:hh\\:mm tt}</p>
        </div>

        <p>Nurse {nurseName} will assist Dr. {doctorName} during your appointment.</p>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "Nurse Assigned to Your Appointment - MediBook", body);
        }

        private async Task<bool> SendNurseAssignedDoctorEmail(string email, string doctorName, string nurseName, string patientName, DateTime date, TimeSpan time, string appointmentId)
        {
            string header = "Nurse Assigned to Your Appointment";
            string salutation = $"Dear Dr. {doctorName},";
            string mainContent = $@"
        <p>A nurse has been assigned to assist you with your upcoming appointment.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Patient:</strong> {patientName}</p>
            <p><strong>Assigned Nurse:</strong> {nurseName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
            <p><strong>Time:</strong> {time:hh\\:mm tt}</p>
        </div>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "Nurse Assigned to Your Appointment - MediBook", body);
        }

        private async Task<bool> SendNurseAssignedNurseEmail(string email, string nurseName, string doctorName, string patientName, DateTime date, TimeSpan time, string appointmentId)
        {
            string header = "You've Been Assigned to an Appointment";
            string salutation = $"Dear {nurseName},";
            string mainContent = $@"
        <p>You have been assigned to assist with an upcoming appointment.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Doctor:</strong> Dr. {doctorName}</p>
            <p><strong>Patient:</strong> {patientName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
            <p><strong>Time:</strong> {time:hh\\:mm tt}</p>
        </div>

        <p>Please review the patient's file and coordinate with Dr. {doctorName} before the appointment.</p>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "New Assignment - MediBook", body);
        }

        private async Task<bool> SendAppointmentClosedWithNursePatientEmail(string email, string patientName, string doctorName, string nurseName, DateTime date, string appointmentId, string notes)
        {
            string header = "Appointment Completed";
            string salutation = $"Dear {patientName},";
            string mainContent = $@"
        <p>Your appointment with <strong>Dr. {doctorName}</strong> and <strong>{nurseName}</strong> on {date:dddd, MMMM dd, yyyy} has been completed.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Doctor:</strong> Dr. {doctorName}</p>
            <p><strong>Nurse:</strong> {nurseName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
        </div>";

            string additionalInfo = string.IsNullOrEmpty(notes) ? "" : $@"
        <p><strong>Notes:</strong></p>
        <div class='appointment-details'>
            <p>{notes}</p>
        </div>";

            string body = GetEmailTemplate(header, salutation, mainContent, additionalInfo);
            return await SendEmailAsync(email, "Appointment Completed - MediBook", body);
        }

        private async Task<bool> SendAppointmentClosedWithNurseDoctorEmail(string email, string doctorName, string patientName, string nurseName, DateTime date, string appointmentId, string notes)
        {
            string header = "Appointment Marked as Completed";
            string salutation = $"Dear Dr. {doctorName},";
            string mainContent = $@"
        <p>Your appointment with <strong>{patientName}</strong> assisted by <strong>{nurseName}</strong> on {date:dddd, MMMM dd, yyyy} has been marked as completed.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Patient:</strong> {patientName}</p>
            <p><strong>Assisting Nurse:</strong> {nurseName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
        </div>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "Appointment Completed - MediBook", body);
        }

        private async Task<bool> SendAppointmentClosedWithNurseNurseEmail(string email, string nurseName, string doctorName, string patientName, DateTime date, string appointmentId, string notes)
        {
            string header = "Appointment Completed";
            string salutation = $"Dear {nurseName},";
            string mainContent = $@"
        <p>The appointment you assisted with has been marked as completed.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Doctor:</strong> Dr. {doctorName}</p>
            <p><strong>Patient:</strong> {patientName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
        </div>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "Appointment Completed - MediBook", body);
        }

        private async Task<bool> SendFeedbackPatientEmail(string email, string patientName, string doctorName, DateTime date, string appointmentId, string feedback, int rating)
        {
            string header = "Thank You for Your Feedback";
            string salutation = $"Dear {patientName},";

            string stars = new string('★', rating) + new string('☆', 5 - rating);

            string mainContent = $@"
        <p>Thank you for providing feedback for your appointment with <strong>Dr. {doctorName}</strong>.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Doctor:</strong> Dr. {doctorName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
            <p><strong>Rating:</strong> {stars} ({rating}/5)</p>
            <p><strong>Your Feedback:</strong> {feedback}</p>
        </div>

        <p>Your feedback helps us improve our services. Dr. {doctorName} may respond to your feedback.</p>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "Feedback Received - MediBook", body);
        }

        private async Task<bool> SendFeedbackDoctorEmail(string email, string doctorName, string patientName, DateTime date, string appointmentId, string feedback, int rating)
        {
            string header = "New Patient Feedback Received";
            string salutation = $"Dear Dr. {doctorName},";

            string stars = new string('★', rating) + new string('☆', 5 - rating);

            string mainContent = $@"
        <p>You have received new feedback from a patient.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Patient:</strong> {patientName}</p>
            <p><strong>Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
            <p><strong>Rating:</strong> {stars} ({rating}/5)</p>
            <p><strong>Feedback:</strong> {feedback}</p>
        </div>

        <p>You can reply to this feedback through the MediBook system.</p>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "New Patient Feedback - MediBook", body);
        }

        private async Task<bool> SendDoctorReplyPatientEmail(string email, string patientName, string doctorName, DateTime date, string appointmentId, string replyMessage)
        {
            string header = "Doctor's Response to Your Feedback";
            string salutation = $"Dear {patientName},";

            string mainContent = $@"
        <p><strong>Dr. {doctorName}</strong> has responded to your feedback.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Doctor:</strong> Dr. {doctorName}</p>
            <p><strong>Appointment Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
        </div>

        <div class='appointment-details'>
            <p><strong>Doctor's Response:</strong></p>
            <p>{replyMessage}</p>
        </div>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "Doctor's Response to Your Feedback - MediBook", body);
        }

        private async Task<bool> SendDoctorReplyDoctorEmail(string email, string doctorName, string patientName, DateTime date, string appointmentId, string replyMessage)
        {
            string header = "Your Response Has Been Sent";
            string salutation = $"Dear Dr. {doctorName},";

            string mainContent = $@"
        <p>Your response to <strong>{patientName}'s</strong> feedback has been sent.</p>
        
        <div class='appointment-details'>
            <p><strong>Appointment ID:</strong> {appointmentId}</p>
            <p><strong>Patient:</strong> {patientName}</p>
            <p><strong>Appointment Date:</strong> {date:dddd, MMMM dd, yyyy}</p>
            <p><strong>Your Response:</strong> {replyMessage}</p>
        </div>";

            string body = GetEmailTemplate(header, salutation, mainContent);
            return await SendEmailAsync(email, "Feedback Response Sent - MediBook", body);
        }

    }
}