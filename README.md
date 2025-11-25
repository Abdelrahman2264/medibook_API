# API Endpoints Documentation

## Rooms Endpoints

| Action               | Method | Route                      | HTTPS                                         | HTTP                                         |
| -------------------- | ------ | -------------------------- | --------------------------------------------- | -------------------------------------------- |
| Get all rooms        | GET    | `/api/Rooms/all`           | `https://localhost:7281/api/Rooms/all`        | `http://localhost:5262/api/Rooms/all`        |
| Get all active rooms | GET    | `/api/Rooms/active`        | `https://localhost:7281/api/Rooms/active`     | `http://localhost:5262/api/Rooms/active`     |
| Get room by ID       | GET    | `/api/Rooms/{id}`          | `https://localhost:7281/api/Rooms/5`          | `http://localhost:5262/api/Rooms/5`          |
| Create a room        | POST   | `/api/Rooms/create`        | `https://localhost:7281/api/Rooms/create`     | `http://localhost:5262/api/Rooms/create`     |
| Activate a room      | PUT    | `/api/Rooms/active/{id}`   | `https://localhost:7281/api/Rooms/active/5`   | `http://localhost:5262/api/Rooms/active/5`   |
| Inactivate a room    | PUT    | `/api/Rooms/inactive/{id}` | `https://localhost:7281/api/Rooms/inactive/5` | `http://localhost:5262/api/Rooms/inactive/5` |
| Update a room        | PUT    | `/api/Rooms/update/{id}`   | `https://localhost:7281/api/Rooms/update/5`   | `http://localhost:5262/api/Rooms/update/5`   |
| Delete a room        | DELETE | `/api/Rooms/delete/{id}`   | `https://localhost:7281/api/Rooms/delete/5`   | `http://localhost:5262/api/Rooms/delete/5`   |

## Users Endpoints

| Action                  | Method | Route                         | HTTPS                                            | HTTP                                           |
| ----------------------- | ------ | ----------------------------- | ------------------------------------------------ | ---------------------------------------------- |
| Get all users           | GET    | `/api/Users/all`              | `https://localhost:7281/api/Users/all`           | `http://localhost:5262/api/Users/all`          |
| Get all active users    | GET    | `/api/Users/active`           | `https://localhost:7281/api/Users/active`        | `http://localhost:5262/api/Users/active`       |
| Get user by ID          | GET    | `/api/Users/{id}`             | `https://localhost:7281/api/Users/5`             | `http://localhost:5262/api/Users/5`            |
| Get current user        | GET    | `/api/Users/current`          | `https://localhost:7281/api/Users/current`       | `http://localhost:5262/api/Users/current`      |
| Create a user           | POST   | `/api/Users/create`           | `https://localhost:7281/api/Users/create`        | `http://localhost:5262/api/Users/create`       |
| Update a user           | PUT    | `/api/Users/update/{id}`      | `https://localhost:7281/api/Users/update/5`      | `http://localhost:5262/api/Users/update/5`     |
| Activate a user         | PATCH  | `/api/Users/{id}/activate`    | `https://localhost:7281/api/Users/5/activate`    | `http://localhost:5262/api/Users/5/activate`   |
| Deactivate a user       | PATCH  | `/api/Users/{id}/deactivate`  | `https://localhost:7281/api/Users/5/deactivate`  | `http://localhost:5262/api/Users/5/deactivate` |

## Roles Endpoints

| Action            | Method | Route                   | HTTPS                                       | HTTP                                      |
| ----------------- | ------ | ----------------------- | ------------------------------------------- | ----------------------------------------- |
| Get all roles     | GET    | `/api/Roles/all`        | `https://localhost:7281/api/Roles/all`      | `http://localhost:5262/api/Roles/all`     |
| Get role by ID    | GET    | `/api/Roles/{id}`       | `https://localhost:7281/api/Roles/5`        | `http://localhost:5262/api/Roles/5`       |
| Create a role     | POST   | `/api/Roles/create`     | `https://localhost:7281/api/Roles/create`   | `http://localhost:5262/api/Roles/create`  |

## Nurses Endpoints

| Action                  | Method | Route                         | HTTPS                                            | HTTP                                           |
| ----------------------- | ------ | ----------------------------- | ------------------------------------------------ | ---------------------------------------------- |
| Get all nurses          | GET    | `/api/Nurses/all`             | `https://localhost:7281/api/Nurses/all`          | `http://localhost:5262/api/Nurses/all`         |
| Get all active nurses   | GET    | `/api/Nurses/active`          | `https://localhost:7281/api/Nurses/active`       | `http://localhost:5262/api/Nurses/active`      |
| Get nurse by ID         | GET    | `/api/Nurses/{id}`            | `https://localhost:7281/api/Nurses/5`            | `http://localhost:5262/api/Nurses/5`           |
| Create a nurse          | POST   | `/api/Nurses/create`          | `https://localhost:7281/api/Nurses/create`       | `http://localhost:5262/api/Nurses/create`      |
| Update a nurse          | PUT    | `/api/Nurses/update/{id}`     | `https://localhost:7281/api/Nurses/update/5`     | `http://localhost:5262/api/Nurses/update/5`    |

## Doctors Endpoints

| Action                  | Method | Route                         | HTTPS                                            | HTTP                                           |
| ----------------------- | ------ | ----------------------------- | ------------------------------------------------ | ---------------------------------------------- |
| Get all doctors         | GET    | `/api/Doctors/all`            | `https://localhost:7281/api/Doctors/all`         | `http://localhost:5262/api/Doctors/all`        |
| Get all active doctors  | GET    | `/api/Doctors/active`         | `https://localhost:7281/api/Doctors/active`      | `http://localhost:5262/api/Doctors/active`     |
| Get doctor by ID        | GET    | `/api/Doctors/{id}`           | `https://localhost:7281/api/Doctors/5`           | `http://localhost:5262/api/Doctors/5`          |
| Create a doctor         | POST   | `/api/Doctors/create`         | `https://localhost:7281/api/Doctors/create`      | `http://localhost:5262/api/Doctors/create`     |
| Update a doctor         | PUT    | `/api/Doctors/update/{id}`    | `https://localhost:7281/api/Doctors/update/5`    | `http://localhost:5262/api/Doctors/update/5`   |

## Authentication Endpoints

| Action                  | Method | Route                             | HTTPS                                                | HTTP                                               |
| ----------------------- | ------ | --------------------------------- | ---------------------------------------------------- | -------------------------------------------------- |
| Login                   | POST   | `/api/Auth/login`                 | `https://localhost:7281/api/Auth/login`              | `http://localhost:5262/api/Auth/login`             |
| Sign In                 | POST   | `/api/Auth/signIn`                | `https://localhost:7281/api/Auth/signIn`             | `http://localhost:5262/api/Auth/signIn`            |
| Sign Out                | POST   | `/api/Auth/signOut`               | `https://localhost:7281/api/Auth/signOut`            | `http://localhost:5262/api/Auth/signOut`           |
| Logout                  | POST   | `/api/Auth/logout`                | `https://localhost:7281/api/Auth/logout`             | `http://localhost:5262/api/Auth/logout`            |
| Send verification       | POST   | `/api/Auth/send-verification`     | `https://localhost:7281/api/Auth/send-verification`  | `http://localhost:5262/api/Auth/send-verification` |
| Forget password         | POST   | `/api/Auth/forget-password`       | `https://localhost:7281/api/Auth/forget-password`    | `http://localhost:5262/api/Auth/forget-password`   |

## Logs Endpoints

| Action                      | Method | Route                             | HTTPS                                                | HTTP                                               |
| --------------------------- | ------ | --------------------------------- | ---------------------------------------------------- | -------------------------------------------------- |
| Get all logs                | GET    | `/api/Logs/all`                   | `https://localhost:7281/api/Logs/all`                | `http://localhost:5262/api/Logs/all`               |
| Get log by ID               | GET    | `/api/Logs/{id}`                  | `https://localhost:7281/api/Logs/5`                  | `http://localhost:5262/api/Logs/5`                 |
| Get logs by user ID         | GET    | `/api/Logs/user/{userId}`         | `https://localhost:7281/api/Logs/user/5`             | `http://localhost:5262/api/Logs/user/5`            |
| Get logs by type            | GET    | `/api/Logs/type/{logType}`        | `https://localhost:7281/api/Logs/type/error`         | `http://localhost:5262/api/Logs/type/error`        |
| Get logs by date range      | GET    | `/api/Logs/date-range`            | `https://localhost:7281/api/Logs/date-range`         | `http://localhost:5262/api/Logs/date-range`        |
| Get current user logs       | GET    | `/api/Logs/current-user`          | `https://localhost:7281/api/Logs/current-user`       | `http://localhost:5262/api/Logs/current-user`      |

## Appointments Endpoints

| Action                          | Method | Route                                     | HTTPS                                                        | HTTP                                                       |
| ------------------------------- | ------ | ----------------------------------------- | ------------------------------------------------------------ | ---------------------------------------------------------- |
| Get all appointments            | GET    | `/api/Appointments/all`                   | `https://localhost:7281/api/Appointments/all`                | `http://localhost:5262/api/Appointments/all`               |
| Get appointment by ID           | GET    | `/api/Appointments/{id}`                  | `https://localhost:7281/api/Appointments/5`                  | `http://localhost:5262/api/Appointments/5`                 |
| Create appointment              | POST   | `/api/Appointments/create`                | `https://localhost:7281/api/Appointments/create`             | `http://localhost:5262/api/Appointments/create`            |
| Assign appointment              | PUT    | `/api/Appointments/assign`                | `https://localhost:7281/api/Appointments/assign`             | `http://localhost:5262/api/Appointments/assign`            |
| Cancel appointment              | PUT    | `/api/Appointments/cancel`                | `https://localhost:7281/api/Appointments/cancel`             | `http://localhost:5262/api/Appointments/cancel`            |
| Close appointment               | PUT    | `/api/Appointments/close`                 | `https://localhost:7281/api/Appointments/close`              | `http://localhost:5262/api/Appointments/close`             |
| Get appointments by patient     | GET    | `/api/Appointments/patient/{id}`          | `https://localhost:7281/api/Appointments/patient/5`          | `http://localhost:5262/api/Appointments/patient/5`         |
| Get appointments by doctor      | GET    | `/api/Appointments/doctor/{id}`           | `https://localhost:7281/api/Appointments/doctor/5`           | `http://localhost:5262/api/Appointments/doctor/5`          |
| Get appointments by nurse       | GET    | `/api/Appointments/nurse/{id}`            | `https://localhost:7281/api/Appointments/nurse/5`            | `http://localhost:5262/api/Appointments/nurse/5`           |
| Get available dates             | GET    | `/api/Appointments/available-dates`       | `https://localhost:7281/api/Appointments/available-dates`    | `http://localhost:5262/api/Appointments/available-dates`   |
| Check date availability         | GET    | `/api/Appointments/check-date`            | `https://localhost:7281/api/Appointments/check-date`         | `http://localhost:5262/api/Appointments/check-date`        |

## Feedbacks Endpoints

| Action                          | Method | Route                                     | HTTPS                                                        | HTTP                                                       |
| ------------------------------- | ------ | ----------------------------------------- | ------------------------------------------------------------ | ---------------------------------------------------------- |
| Get all feedbacks               | GET    | `/api/Feedbacks/all`                      | `https://localhost:7281/api/Feedbacks/all`                   | `http://localhost:5262/api/Feedbacks/all`                  |
| Get feedback by ID              | GET    | `/api/Feedbacks/{id}`                     | `https://localhost:7281/api/Feedbacks/5`                     | `http://localhost:5262/api/Feedbacks/5`                    |
| Create feedback                 | POST   | `/api/Feedbacks/create`                   | `https://localhost:7281/api/Feedbacks/create`                | `http://localhost:5262/api/Feedbacks/create`               |
| Add doctor reply                | PUT    | `/api/Feedbacks/add-doctor-reply`         | `https://localhost:7281/api/Feedbacks/add-doctor-reply`      | `http://localhost:5262/api/Feedbacks/add-doctor-reply`     |
| Update feedback                 | PUT    | `/api/Feedbacks/update`                   | `https://localhost:7281/api/Feedbacks/update`                | `http://localhost:5262/api/Feedbacks/update`               |
| Delete feedback                 | DELETE | `/api/Feedbacks/{id}`                     | `https://localhost:7281/api/Feedbacks/5`                     | `http://localhost:5262/api/Feedbacks/5`                    |
| Toggle favourite                | PATCH  | `/api/Feedbacks/{id}/toggle-favourite`    | `https://localhost:7281/api/Feedbacks/5/toggle-favourite`    | `http://localhost:5262/api/Feedbacks/5/toggle-favourite`   |
| Get feedbacks by doctor         | GET    | `/api/Feedbacks/doctor/{doctorId}`        | `https://localhost:7281/api/Feedbacks/doctor/5`              | `http://localhost:5262/api/Feedbacks/doctor/5`             |
| Get feedbacks by patient        | GET    | `/api/Feedbacks/patient/{patientId}`      | `https://localhost:7281/api/Feedbacks/patient/5`             | `http://localhost:5262/api/Feedbacks/patient/5`            |
| Get feedbacks by nurse          | GET    | `/api/Feedbacks/nurse/{nurseId}`          | `https://localhost:7281/api/Feedbacks/nurse/5`               | `http://localhost:5262/api/Feedbacks/nurse/5`              |

## Notifications Endpoints

| Action                                  | Method | Route                                             | HTTPS                                                                | HTTP                                                               |
| --------------------------------------- | ------ | ------------------------------------------------- | -------------------------------------------------------------------- | ------------------------------------------------------------------ |
| Get all notifications                   | GET    | `/api/Notifications/all`                          | `https://localhost:7281/api/Notifications/all`                       | `http://localhost:5262/api/Notifications/all`                      |
| Get notification by ID                  | GET    | `/api/Notifications/{id}`                         | `https://localhost:7281/api/Notifications/5`                         | `http://localhost:5262/api/Notifications/5`                        |
| Get notifications by user ID            | GET    | `/api/Notifications/user/{userId}`                | `https://localhost:7281/api/Notifications/user/5`                    | `http://localhost:5262/api/Notifications/user/5`                   |
| Get current user notifications          | GET    | `/api/Notifications/current-user`                 | `https://localhost:7281/api/Notifications/current-user`              | `http://localhost:5262/api/Notifications/current-user`             |
| Get unread notifications by user        | GET    | `/api/Notifications/user/{userId}/unread`         | `https://localhost:7281/api/Notifications/user/5/unread`             | `http://localhost:5262/api/Notifications/user/5/unread`            |
| Get current user unread notifications   | GET    | `/api/Notifications/current-user/unread`          | `https://localhost:7281/api/Notifications/current-user/unread`       | `http://localhost:5262/api/Notifications/current-user/unread`      |
| Get read notifications by user          | GET    | `/api/Notifications/user/{userId}/read`           | `https://localhost:7281/api/Notifications/user/5/read`               | `http://localhost:5262/api/Notifications/user/5/read`              |
| Get notifications by user and status    | GET    | `/api/Notifications/user/{userId}/status/{isRead}`| `https://localhost:7281/api/Notifications/user/5/status/true`        | `http://localhost:5262/api/Notifications/user/5/status/true`       |
| Get unread count for current user       | GET    | `/api/Notifications/current-user/unread-count`    | `https://localhost:7281/api/Notifications/current-user/unread-count` | `http://localhost:5262/api/Notifications/current-user/unread-count`|
| Get unread count by user ID             | GET    | `/api/Notifications/user/{userId}/unread-count`   | `https://localhost:7281/api/Notifications/user/5/unread-count`       | `http://localhost:5262/api/Notifications/user/5/unread-count`      |
| Mark notification as read               | PATCH  | `/api/Notifications/{id}/mark-read`               | `https://localhost:7281/api/Notifications/5/mark-read`               | `http://localhost:5262/api/Notifications/5/mark-read`              |
| Mark all notifications as read          | PATCH  | `/api/Notifications/current-user/mark-all-read`   | `https://localhost:7281/api/Notifications/current-user/mark-all-read`| `http://localhost:5262/api/Notifications/current-user/mark-all-read`|
| Mark all notifications as read by user  | PATCH  | `/api/Notifications/user/{userId}/mark-all-read`  | `https://localhost:7281/api/Notifications/user/5/mark-all-read`      | `http://localhost:5262/api/Notifications/user/5/mark-all-read`     |

## Base URLs
- **HTTPS**: `https://localhost:7281`
- **HTTP**: `http://localhost:5262`

## Notes
- Replace `{id}`, `{userId}`, `{logType}`, `{doctorId}`, `{patientId}`, `{nurseId}` with actual values in the URL
- All POST, PUT, and PATCH endpoints require appropriate request body data
- Most endpoints require JWT authentication (marked with `[Authorize]` attribute)
- Responses include appropriate HTTP status codes and error messages
- Date range endpoints may require query parameters for start and end dates
- Appointment check-date endpoint requires query parameter: `?time=2025-11-25T10:00:00`