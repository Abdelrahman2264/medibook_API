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
    }
}
