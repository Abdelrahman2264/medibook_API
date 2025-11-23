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

| Action            | Method | Route                 | HTTPS                                      | HTTP                                     |
| ----------------- | ------ | --------------------- | ------------------------------------------ | ---------------------------------------- |
| Login             | POST   | `/api/Auth/login`     | `https://localhost:7281/api/Auth/login`    | `http://localhost:5262/api/Auth/login`   |
| Logout            | POST   | `/api/Auth/logout`    | `https://localhost:7281/api/Auth/logout`   | `http://localhost:5262/api/Auth/logout`  |

## Base URLs
- **HTTPS**: `https://localhost:7281`
- **HTTP**: `http://localhost:5262`

## Notes
- Replace `{id}` with the actual ID number in the URL
- All POST, PUT, and PATCH endpoints require appropriate request body data
- Authentication may be required for certain endpoints
- Responses include appropriate HTTP status codes and error messages