# Rooms Endpoints

This document describes the API endpoints available for managing rooms.

## Endpoints

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