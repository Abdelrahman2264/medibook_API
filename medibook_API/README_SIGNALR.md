# SignalR Real-Time Implementation Guide

This document explains how to use SignalR for real-time notifications and updates in the Medibook API.

## Overview

SignalR has been integrated to provide real-time functionality across the application. When actions are performed (create, update, delete), notifications are sent instantly to connected clients.

## Architecture

### Backend Components

1. **NotificationHub** (`Hubs/NotificationHub.cs`)
   - SignalR hub that manages client connections
   - Groups users by their user ID for targeted notifications
   - Requires JWT authentication

2. **SignalRService** (`Extensions/Services/SignalRService.cs`)
   - Service for sending notifications and updates via SignalR
   - Methods:
     - `SendNotificationToUserAsync`: Send notification to specific user
     - `SendNotificationToUsersAsync`: Send notification to multiple users
     - `SendRealTimeUpdateAsync`: Send real-time update (for CRUD operations)
     - `BroadcastUpdateAsync`: Broadcast update to all connected clients

3. **SignalRHelper** (`Extensions/Helpers/SignalRHelper.cs`)
   - Helper class with convenience methods for common operations
   - Methods:
     - `NotifyCreatedAsync`: Notify when entity is created
     - `NotifyUpdatedAsync`: Notify when entity is updated
     - `NotifyDeletedAsync`: Notify when entity is deleted

### Frontend Components

1. **SignalRService** (`services/signalr.service.ts`)
   - Angular service that manages SignalR connection
   - Automatically connects when user is authenticated
   - Provides observables for notifications and updates

2. **NotificationCardComponent** (`components/notification-card/notification-card.component.ts`)
   - Displays real-time notification cards with sound
   - Auto-closes after 5 seconds
   - Shows notification icon based on message type

## Usage Examples

### Example 1: Send Real-Time Update After Creating an Appointment

```csharp
[HttpPost("create")]
public async Task<IActionResult> CreateAppointment([FromBody] CreateAppoinmentDto dto)
{
    try
    {
        var response = await appointmentRepository.CreateAppintmentAsync(dto);
        
        if (response.appointment_id > 0)
        {
            // Send real-time update
            await SignalRHelper.NotifyCreatedAsync(
                _signalRService,
                "Appointment",
                new { 
                    AppointmentId = response.appointment_id,
                    PatientId = dto.patient_id,
                    DoctorId = dto.doctor_id,
                    AppointmentDate = response.appointment_date
                }
            );
        }
        
        return CreatedAtAction(nameof(GetAppointmentById), new { id = response.appointment_id }, response);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error creating appointment.");
        return StatusCode(500, "Internal server error");
    }
}
```

### Example 2: Send Real-Time Update After Updating a User

```csharp
[HttpPut("update/{id:int}")]
public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
{
    try
    {
        var updatedUser = await userRepository.UpdateUserAsync(id, dto);
        
        if (updatedUser != null)
        {
            // Send real-time update to the user who was updated
            await SignalRHelper.NotifyUpdatedAsync(
                _signalRService,
                "User",
                new { 
                    UserId = updatedUser.UserId,
                    FirstName = updatedUser.FirstName,
                    LastName = updatedUser.LastName
                },
                updatedUser.UserId // Send to specific user
            );
        }
        
        return Ok(updatedUser);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error updating user.");
        return StatusCode(500, "Internal server error");
    }
}
```

### Example 3: Send Real-Time Update After Deleting an Entity

```csharp
[HttpDelete("{id:int}")]
public async Task<IActionResult> DeleteEntity(int id)
{
    try
    {
        var success = await repository.DeleteEntityAsync(id);
        
        if (success)
        {
            // Broadcast deletion to all connected clients
            await SignalRHelper.NotifyDeletedAsync(
                _signalRService,
                "Entity",
                id
            );
        }
        
        return Ok(new { Message = "Entity deleted successfully." });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error deleting entity.");
        return StatusCode(500, "Internal server error");
    }
}
```

### Example 4: Inject SignalRService in Controller

```csharp
public class YourController : Controller
{
    private readonly ISignalRService _signalRService;
    
    public YourController(
        ILogger<YourController> logger,
        IYourRepository repository,
        ISignalRService signalRService) // Inject SignalRService
    {
        _logger = logger;
        _repository = repository;
        _signalRService = signalRService;
    }
    
    // Use _signalRService in your methods
}
```

## Frontend Usage

### Listening to Real-Time Updates

The frontend automatically listens to SignalR updates. Components can subscribe to updates:

```typescript
constructor(private signalRService: SignalRService) {}

ngOnInit() {
  // Listen to real-time updates
  this.signalRService.update$.subscribe(update => {
    console.log('Update received:', update);
    
    // Handle different update types
    if (update.type === 'Appointment_Created') {
      // Refresh appointments list
      this.loadAppointments();
    } else if (update.type === 'User_Updated') {
      // Refresh users list
      this.loadUsers();
    }
  });
}
```

### Notification Cards

Notification cards are automatically displayed when notifications are received via SignalR. The cards:
- Play a sound (if `assets/sounds/notification.mp3` exists)
- Auto-close after 5 seconds
- Can be manually closed by clicking the × button
- Stack vertically if multiple notifications arrive

## Configuration

### Backend

SignalR is configured in `Program.cs`:
- Hub endpoint: `/notificationhub`
- JWT authentication is required
- CORS is enabled for SignalR connections

### Frontend

SignalR service is configured in `services/signalr.service.ts`:
- Connection URL: `http://localhost:5262/notificationhub`
- Automatically reconnects on connection loss
- Uses JWT token from `AuthService`

## Sound File

To enable notification sounds:
1. Place a sound file at `medibook_FrontEnd/src/assets/sounds/notification.mp3`
2. Supported formats: MP3, WAV, OGG
3. If the file is not found, notifications will still display without sound

## Best Practices

1. **Always handle errors**: Wrap SignalR calls in try-catch blocks
2. **Don't block operations**: SignalR calls should not block the main operation
3. **Use appropriate update types**: Use descriptive update type names (e.g., `Appointment_Created`, `User_Updated`)
4. **Target specific users when possible**: Use the `userId` parameter to send updates only to relevant users
5. **Broadcast sparingly**: Only broadcast to all users when the update is relevant to everyone

## Troubleshooting

### Connection Issues

- Check that JWT token is valid
- Verify SignalR hub endpoint is accessible
- Check browser console for connection errors
- Ensure CORS is properly configured

### Notifications Not Appearing

- Verify SignalR connection is established (check browser console)
- Check that notification is being sent from backend
- Verify user is in the correct SignalR group
- Check that frontend is subscribed to notification observable

### Sound Not Playing

- Verify sound file exists at correct path
- Check browser console for audio errors
- Ensure browser allows autoplay (some browsers block autoplay)
- Try a different audio format (MP3, WAV, OGG)

