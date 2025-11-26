# Medibook API - Backend Documentation

A comprehensive healthcare management system API built with ASP.NET Core 9.0, providing endpoints for managing users, appointments, doctors, nurses, feedbacks, notifications, and more.

## Table of Contents

- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Configuration](#configuration)
- [Database](#database)
- [API Endpoints](#api-endpoints)
- [Authentication](#authentication)
- [Swagger Documentation](#swagger-documentation)

## Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server (Entity Framework Core)
- **Authentication**: JWT Bearer Tokens
- **API Documentation**: Swagger/OpenAPI
- **Password Hashing**: BCrypt.Net-Next
- **Email Service**: SMTP (configurable)

## Prerequisites

- .NET 9.0 SDK or later
- SQL Server (LocalDB, Express, or Full version)
- Visual Studio 2022, VS Code, or Rider (optional)

## Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd medibook_API
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure the database connection**
   - Update `appsettings.json` or `appsettings.Development.json` with your SQL Server connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MedibookDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

4. **Configure JWT settings**
   - Add JWT configuration to `appsettings.json`:
   ```json
   {
     "Jwt": {
       "Key": "YourSecretKeyHere_MustBeAtLeast32Characters",
       "Issuer": "MedibookAPI",
       "Audience": "MedibookUsers"
     }
   }
   ```

5. **Configure Email settings** (for password reset and verification)
   ```json
   {
     "EmailSettings": {
       "SmtpServer": "smtp.gmail.com",
       "SmtpPort": 587,
       "SenderEmail": "your-email@gmail.com",
       "SenderPassword": "your-app-password",
       "SenderName": "Medibook System"
     }
   }
   ```

6. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

7. **Run the application**
   ```bash
   dotnet run
   ```

8. **Access Swagger UI**
   - Navigate to `https://localhost:7281` (HTTPS) or `http://localhost:5262` (HTTP)
   - Swagger UI will be available at the root URL

## Configuration

### Connection Strings
- **Development**: Update `appsettings.Development.json`
- **Production**: Update `appsettings.Production.json`

### JWT Authentication
- JWT tokens are used for securing API endpoints
- Tokens are validated on each request for protected endpoints
- Token expiration and validation settings are configured in `Program.cs`

### CORS
- Currently configured to allow all origins, headers, and methods
- Update in `Program.cs` for production environments

## Database

The application uses Entity Framework Core with SQL Server. The database context is defined in `Medibook_Context.cs`.

### Models
- **Users**: Patient and staff user accounts
- **Doctors**: Doctor profiles and information
- **Nurses**: Nurse profiles and information
- **Appointments**: Medical appointments scheduling
- **Feedbacks**: Patient feedback and doctor replies
- **Notifications**: System notifications for users
- **Rooms**: Hospital/clinic room management
- **Roles**: User role definitions
- **Logs**: System activity logs

### Migrations
- Initial migration: `20251123170404_INITDATABASE`
- Run migrations: `dotnet ef database update`
- Create new migration: `dotnet ef migrations add MigrationName`

## API Endpoints

### Base URLs
- **HTTPS**: `https://localhost:7281`
- **HTTP**: `http://localhost:5262`

---

## Authentication Endpoints

| Action | Method | Route | Description |
|--------|--------|-------|-------------|
| Sign In | POST | `/api/Auth/signIn` | Authenticate user and receive JWT token |
| Sign Out | POST | `/api/Auth/SignOut` | Logout user (client-side token removal) |
| Send Verification Code | POST | `/api/Auth/send-verification` | Send email verification code for signup |
| Check Email (Forget Password) | POST | `/api/Auth/check-email` | Check if email exists and send reset code |
| Verify Forget Password Code | POST | `/api/Auth/verify-forget-password-code` | Verify the reset code sent to email |
| Reset Password | POST | `/api/Auth/reset-password` | Reset password after code verification |
| Forget Password | POST | `/api/Auth/forget-password` | Legacy endpoint for password reset |

### Authentication Request/Response Examples

**Sign In Request:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Sign In Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Login successful",
  "user": {
    "userId": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

**Check Email Request (Forget Password):**
```json
{
  "email": "user@example.com"
}
```

**Check Email Response:**
```json
{
  "success": true,
  "message": "Verification code sent successfully",
  "email": "user@example.com",
  "userId": 1
}
```

**Verify Forget Password Code Request:**
```json
{
  "email": "user@example.com",
  "code": "123456"
}
```

**Reset Password Request:**
```json
{
  "email": "user@example.com",
  "newPassword": "newPassword123",
  "confirmPassword": "newPassword123"
}
```

---

## Rooms Endpoints

| Action | Method | Route | HTTPS | HTTP |
|--------|--------|-------|-------|------|
| Get all rooms | GET | `/api/Rooms/all` | `https://localhost:7281/api/Rooms/all` | `http://localhost:5262/api/Rooms/all` |
| Get all active rooms | GET | `/api/Rooms/active` | `https://localhost:7281/api/Rooms/active` | `http://localhost:5262/api/Rooms/active` |
| Get room by ID | GET | `/api/Rooms/{id}` | `https://localhost:7281/api/Rooms/5` | `http://localhost:5262/api/Rooms/5` |
| Create a room | POST | `/api/Rooms/create` | `https://localhost:7281/api/Rooms/create` | `http://localhost:5262/api/Rooms/create` |
| Activate a room | PUT | `/api/Rooms/active/{id}` | `https://localhost:7281/api/Rooms/active/5` | `http://localhost:5262/api/Rooms/active/5` |
| Inactivate a room | PUT | `/api/Rooms/inactive/{id}` | `https://localhost:7281/api/Rooms/inactive/5` | `http://localhost:5262/api/Rooms/inactive/5` |
| Update a room | PUT | `/api/Rooms/update/{id}` | `https://localhost:7281/api/Rooms/update/5` | `http://localhost:5262/api/Rooms/update/5` |
| Delete a room | DELETE | `/api/Rooms/delete/{id}` | `https://localhost:7281/api/Rooms/delete/5` | `http://localhost:5262/api/Rooms/delete/5` |

## Users Endpoints

| Action | Method | Route | HTTPS | HTTP |
|--------|--------|-------|-------|------|
| Get all users | GET | `/api/Users/all` | `https://localhost:7281/api/Users/all` | `http://localhost:5262/api/Users/all` |
| Get all active users | GET | `/api/Users/active` | `https://localhost:7281/api/Users/active` | `http://localhost:5262/api/Users/active` |
| Get user by ID | GET | `/api/Users/{id}` | `https://localhost:7281/api/Users/5` | `http://localhost:5262/api/Users/5` |
| Get current user | GET | `/api/Users/current` | `https://localhost:7281/api/Users/current` | `http://localhost:5262/api/Users/current` |
| Create a user | POST | `/api/Users/create` | `https://localhost:7281/api/Users/create` | `http://localhost:5262/api/Users/create` |
| Update a user | PUT | `/api/Users/update/{id}` | `https://localhost:7281/api/Users/update/5` | `http://localhost:5262/api/Users/update/5` |
| Activate a user | PATCH | `/api/Users/{id}/activate` | `https://localhost:7281/api/Users/5/activate` | `http://localhost:5262/api/Users/5/activate` |
| Deactivate a user | PATCH | `/api/Users/{id}/deactivate` | `https://localhost:7281/api/Users/5/deactivate` | `http://localhost:5262/api/Users/5/deactivate` |

## Roles Endpoints

| Action | Method | Route | HTTPS | HTTP |
|--------|--------|-------|-------|------|
| Get all roles | GET | `/api/Roles/all` | `https://localhost:7281/api/Roles/all` | `http://localhost:5262/api/Roles/all` |
| Get role by ID | GET | `/api/Roles/{id}` | `https://localhost:7281/api/Roles/5` | `http://localhost:5262/api/Roles/5` |
| Create a role | POST | `/api/Roles/create` | `https://localhost:7281/api/Roles/create` | `http://localhost:5262/api/Roles/create` |

## Nurses Endpoints

| Action | Method | Route | HTTPS | HTTP |
|--------|--------|-------|-------|------|
| Get all nurses | GET | `/api/Nurses/all` | `https://localhost:7281/api/Nurses/all` | `http://localhost:5262/api/Nurses/all` |
| Get all active nurses | GET | `/api/Nurses/active` | `https://localhost:7281/api/Nurses/active` | `http://localhost:5262/api/Nurses/active` |
| Get nurse by ID | GET | `/api/Nurses/{id}` | `https://localhost:7281/api/Nurses/5` | `http://localhost:5262/api/Nurses/5` |
| Create a nurse | POST | `/api/Nurses/create` | `https://localhost:7281/api/Nurses/create` | `http://localhost:5262/api/Nurses/create` |
| Update a nurse | PUT | `/api/Nurses/update/{id}` | `https://localhost:7281/api/Nurses/update/5` | `http://localhost:5262/api/Nurses/update/5` |

## Doctors Endpoints

| Action | Method | Route | HTTPS | HTTP |
|--------|--------|-------|-------|------|
| Get all doctors | GET | `/api/Doctors/all` | `https://localhost:7281/api/Doctors/all` | `http://localhost:5262/api/Doctors/all` |
| Get all active doctors | GET | `/api/Doctors/active` | `https://localhost:7281/api/Doctors/active` | `http://localhost:5262/api/Doctors/active` |
| Get doctor by ID | GET | `/api/Doctors/{id}` | `https://localhost:7281/api/Doctors/5` | `http://localhost:5262/api/Doctors/5` |
| Create a doctor | POST | `/api/Doctors/create` | `https://localhost:7281/api/Doctors/create` | `http://localhost:5262/api/Doctors/create` |
| Update a doctor | PUT | `/api/Doctors/update/{id}` | `https://localhost:7281/api/Doctors/update/5` | `http://localhost:5262/api/Doctors/update/5` |

## Logs Endpoints

| Action | Method | Route | HTTPS | HTTP |
|--------|--------|-------|-------|------|
| Get all logs | GET | `/api/Logs/all` | `https://localhost:7281/api/Logs/all` | `http://localhost:5262/api/Logs/all` |
| Get log by ID | GET | `/api/Logs/{id}` | `https://localhost:7281/api/Logs/5` | `http://localhost:5262/api/Logs/5` |
| Get logs by user ID | GET | `/api/Logs/user/{userId}` | `https://localhost:7281/api/Logs/user/5` | `http://localhost:5262/api/Logs/user/5` |
| Get logs by type | GET | `/api/Logs/type/{logType}` | `https://localhost:7281/api/Logs/type/error` | `http://localhost:5262/api/Logs/type/error` |
| Get logs by date range | GET | `/api/Logs/date-range` | `https://localhost:7281/api/Logs/date-range` | `http://localhost:5262/api/Logs/date-range` |
| Get current user logs | GET | `/api/Logs/current-user` | `https://localhost:7281/api/Logs/current-user` | `http://localhost:5262/api/Logs/current-user` |

## Appointments Endpoints

| Action | Method | Route | HTTPS | HTTP |
|--------|--------|-------|-------|------|
| Get all appointments | GET | `/api/Appointments/all` | `https://localhost:7281/api/Appointments/all` | `http://localhost:5262/api/Appointments/all` |
| Get appointment by ID | GET | `/api/Appointments/{id}` | `https://localhost:7281/api/Appointments/5` | `http://localhost:5262/api/Appointments/5` |
| Create appointment | POST | `/api/Appointments/create` | `https://localhost:7281/api/Appointments/create` | `http://localhost:5262/api/Appointments/create` |
| Assign appointment | PUT | `/api/Appointments/assign` | `https://localhost:7281/api/Appointments/assign` | `http://localhost:5262/api/Appointments/assign` |
| Cancel appointment | PUT | `/api/Appointments/cancel` | `https://localhost:7281/api/Appointments/cancel` | `http://localhost:5262/api/Appointments/cancel` |
| Close appointment | PUT | `/api/Appointments/close` | `https://localhost:7281/api/Appointments/close` | `http://localhost:5262/api/Appointments/close` |
| Get appointments by patient | GET | `/api/Appointments/patient/{id}` | `https://localhost:7281/api/Appointments/patient/5` | `http://localhost:5262/api/Appointments/patient/5` |
| Get appointments by doctor | GET | `/api/Appointments/doctor/{id}` | `https://localhost:7281/api/Appointments/doctor/5` | `http://localhost:5262/api/Appointments/doctor/5` |
| Get appointments by nurse | GET | `/api/Appointments/nurse/{id}` | `https://localhost:7281/api/Appointments/nurse/5` | `http://localhost:5262/api/Appointments/nurse/5` |
| Get available dates | GET | `/api/Appointments/available-dates` | `https://localhost:7281/api/Appointments/available-dates` | `http://localhost:5262/api/Appointments/available-dates` |
| Check date availability | GET | `/api/Appointments/check-date` | `https://localhost:7281/api/Appointments/check-date` | `http://localhost:5262/api/Appointments/check-date` |

## Feedbacks Endpoints

| Action | Method | Route | HTTPS | HTTP |
|--------|--------|-------|-------|------|
| Get all feedbacks | GET | `/api/Feedbacks/all` | `https://localhost:7281/api/Feedbacks/all` | `http://localhost:5262/api/Feedbacks/all` |
| Get feedback by ID | GET | `/api/Feedbacks/{id}` | `https://localhost:7281/api/Feedbacks/5` | `http://localhost:5262/api/Feedbacks/5` |
| Create feedback | POST | `/api/Feedbacks/create` | `https://localhost:7281/api/Feedbacks/create` | `http://localhost:5262/api/Feedbacks/create` |
| Add doctor reply | PUT | `/api/Feedbacks/add-doctor-reply` | `https://localhost:7281/api/Feedbacks/add-doctor-reply` | `http://localhost:5262/api/Feedbacks/add-doctor-reply` |
| Update feedback | PUT | `/api/Feedbacks/update` | `https://localhost:7281/api/Feedbacks/update` | `http://localhost:5262/api/Feedbacks/update` |
| Delete feedback | DELETE | `/api/Feedbacks/{id}` | `https://localhost:7281/api/Feedbacks/5` | `http://localhost:5262/api/Feedbacks/5` |
| Toggle favourite | PATCH | `/api/Feedbacks/{id}/toggle-favourite` | `https://localhost:7281/api/Feedbacks/5/toggle-favourite` | `http://localhost:5262/api/Feedbacks/5/toggle-favourite` |
| Get feedbacks by doctor | GET | `/api/Feedbacks/doctor/{doctorId}` | `https://localhost:7281/api/Feedbacks/doctor/5` | `http://localhost:5262/api/Feedbacks/doctor/5` |
| Get feedbacks by patient | GET | `/api/Feedbacks/patient/{patientId}` | `https://localhost:7281/api/Feedbacks/patient/5` | `http://localhost:5262/api/Feedbacks/patient/5` |
| Get feedbacks by nurse | GET | `/api/Feedbacks/nurse/{nurseId}` | `https://localhost:7281/api/Feedbacks/nurse/5` | `http://localhost:5262/api/Feedbacks/nurse/5` |

## Notifications Endpoints

| Action | Method | Route | HTTPS | HTTP |
|--------|--------|-------|-------|------|
| Get all notifications | GET | `/api/Notifications/all` | `https://localhost:7281/api/Notifications/all` | `http://localhost:5262/api/Notifications/all` |
| Get notification by ID | GET | `/api/Notifications/{id}` | `https://localhost:7281/api/Notifications/5` | `http://localhost:5262/api/Notifications/5` |
| Get notifications by user ID | GET | `/api/Notifications/user/{userId}` | `https://localhost:7281/api/Notifications/user/5` | `http://localhost:5262/api/Notifications/user/5` |
| Get current user notifications | GET | `/api/Notifications/current-user` | `https://localhost:7281/api/Notifications/current-user` | `http://localhost:5262/api/Notifications/current-user` |
| Get unread notifications by user | GET | `/api/Notifications/user/{userId}/unread` | `https://localhost:7281/api/Notifications/user/5/unread` | `http://localhost:5262/api/Notifications/user/5/unread` |
| Get current user unread notifications | GET | `/api/Notifications/current-user/unread` | `https://localhost:7281/api/Notifications/current-user/unread` | `http://localhost:5262/api/Notifications/current-user/unread` |
| Get read notifications by user | GET | `/api/Notifications/user/{userId}/read` | `https://localhost:7281/api/Notifications/user/5/read` | `http://localhost:5262/api/Notifications/user/5/read` |
| Get notifications by user and status | GET | `/api/Notifications/user/{userId}/status/{isRead}` | `https://localhost:7281/api/Notifications/user/5/status/true` | `http://localhost:5262/api/Notifications/user/5/status/true` |
| Get unread count for current user | GET | `/api/Notifications/current-user/unread-count` | `https://localhost:7281/api/Notifications/current-user/unread-count` | `http://localhost:5262/api/Notifications/current-user/unread-count` |
| Get unread count by user ID | GET | `/api/Notifications/user/{userId}/unread-count` | `https://localhost:7281/api/Notifications/user/5/unread-count` | `http://localhost:5262/api/Notifications/user/5/unread-count` |
| Mark notification as read | PATCH | `/api/Notifications/{id}/mark-read` | `https://localhost:7281/api/Notifications/5/mark-read` | `http://localhost:5262/api/Notifications/5/mark-read` |
| Mark all notifications as read | PATCH | `/api/Notifications/current-user/mark-all-read` | `https://localhost:7281/api/Notifications/current-user/mark-all-read` | `http://localhost:5262/api/Notifications/current-user/mark-all-read` |
| Mark all notifications as read by user | PATCH | `/api/Notifications/user/{userId}/mark-all-read` | `https://localhost:7281/api/Notifications/user/5/mark-all-read` | `http://localhost:5262/api/Notifications/user/5/mark-all-read` |

## Authentication

### JWT Token Usage

Most endpoints require JWT authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### Public Endpoints (No Authentication Required)
- `POST /api/Auth/signIn` - User login
- `POST /api/Auth/send-verification` - Send verification code
- `POST /api/Auth/check-email` - Check email for password reset
- `POST /api/Auth/verify-forget-password-code` - Verify reset code
- `POST /api/Auth/reset-password` - Reset password

### Protected Endpoints
All other endpoints require a valid JWT token in the Authorization header.

## Swagger Documentation

When running in Development mode, Swagger UI is available at:
- **HTTPS**: `https://localhost:7281`
- **HTTP**: `http://localhost:5262`

Swagger provides:
- Interactive API testing
- Request/response schemas
- Authentication testing with JWT tokens
- All endpoint documentation

## Notes

- Replace `{id}`, `{userId}`, `{logType}`, `{doctorId}`, `{patientId}`, `{nurseId}` with actual values in the URL
- All POST, PUT, and PATCH endpoints require appropriate request body data
- Most endpoints require JWT authentication (marked with `[Authorize]` attribute)
- Responses include appropriate HTTP status codes and error messages
- Date range endpoints may require query parameters for start and end dates
- Appointment check-date endpoint requires query parameter: `?time=2025-11-25T10:00:00`
- Verification codes for password reset expire after 10 minutes
- Email verification codes are stored in memory (will be lost on server restart)

## Error Handling

The API returns standard HTTP status codes:
- `200 OK` - Request successful
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Missing or invalid JWT token
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

Error responses include a message describing the issue.

## License

[Add your license information here]

## Contributing

[Add contributing guidelines here]
