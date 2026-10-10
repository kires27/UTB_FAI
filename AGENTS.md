# CalendarWebApp - Agent Development Guide

This guide provides comprehensive information for agentic coding assistants working with the CalendarWebApp codebase.

## Project Overview

CalendarWebApp is a clean architecture ASP.NET Core 9.0 application for event management with user authentication and notifications.

**Architecture Layers:**
- `CalendarApp.Domain` - Core entities and validation
- `CalendarApp.Infrastructure` - Data access (EF Core)
- `CalendarApp.Application` - Business logic and services
- `CalendarApp.Web` - ASP.NET Core MVC presentation layer

**Technology Stack:**
- .NET 9.0 with nullable reference types
- Entity Framework Core 9.0.9 with MySQL
- ASP.NET Core Identity for authentication
- Bootstrap 5 + jQuery for frontend
- Clean Architecture with dependency injection

## Build & Development Commands

### Essential Commands
```bash
# Run the application
dotnet run --project CalendarApp.Web

# Run with hot reload for development
dotnet watch run --project CalendarApp.Web

# Build the entire solution
dotnet build CalendarApp.sln

# Clean build artifacts
dotnet clean CalendarApp.sln
```

### Database Operations
```bash
# Add a new migration
dotnet tool run dotnet-ef migrations add MigrationName --project CalendarApp.Infrastructure --startup-project CalendarApp.Web

# Apply migrations to database
dotnet tool run dotnet-ef database update --project CalendarApp.Infrastructure --startup-project CalendarApp.Web

# Drop the database (development only)
dotnet tool run dotnet-ef database drop --project CalendarApp.Infrastructure --startup-project CalendarApp.Web
```

### Testing Commands
**Note: Test framework not yet implemented. Plan to add:**
```bash
# When test framework is added:
dotnet test                           # Run all tests
dotnet test --filter "MethodName"     # Run specific test
dotnet test --logger "console;verbosity=detailed"
```

## Code Style Guidelines

### Naming Conventions
- **Classes**: PascalCase (e.g., `EventAppService`, `CalendarDbContext`)
- **Interfaces**: PascalCase with 'I' prefix (e.g., `IEventAppService`)
- **Methods**: PascalCase (e.g., `SelectAll()`, `Create()`)
- **Properties**: PascalCase (e.g., `StartTime`, `OwnerId`)
- **Fields/Private Variables**: CamelCase with underscore prefix (e.g., `_eventAppService`)
- **Constants**: PascalCase (e.g., `MaxEventTitleLength`)
- **Enums**: PascalCase (e.g., `EventStatus`, `AttendeeRole`)

### Import Organization
```csharp
// System namespaces first
using System;
using System.ComponentModel.DataAnnotations;

// Third-party namespaces
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

// Project namespaces (alphabetical)
using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;
using CalendarApp.Infrastructure.Database;
```

### Formatting Rules
- Use 4 spaces for indentation (no tabs)
- Opening braces on new line for classes/methods
- Opening braces on same line for control structures
- Maximum line length: 120 characters
- One blank line between methods
- Use regions for organizing large classes (optional)

### Nullable Reference Types
```csharp
// Enabled throughout project - always be explicit
public string? Description { get; set; }      // Nullable
public string Title { get; set; } = string.Empty;  // Non-nullable with default
public required string RequiredField { get; set; }  // Required property
```

## Architecture Guidelines

### Clean Architecture Principles
- **Dependency Direction**: Web → Application → Infrastructure → Domain
- **Domain Layer**: No external dependencies, pure business logic
- **Application Layer**: Service interfaces, implementations, and ViewModels (`CalendarApp.Application.Models`)
- **Infrastructure Layer**: EF Core, external services
- **Web Layer**: Controllers, views, `ErrorViewModel`

### Dependency Injection Pattern
```csharp
public class EventController : Controller
{
    private readonly IEventAppService _eventAppService;

    public EventController(IEventAppService eventAppService)
    {
        _eventAppService = eventAppService;
    }
}
```

### Service Layer Pattern
```csharp
// Interface in Application/Abstraction/
public interface IEventAppService
{
    IList<Event> SelectAll();
    Event? GetById(int id);
    void Create(Event @event);
}

// Implementation in Application/Implementation/
public class EventAppService : IEventAppService
{
    private readonly CalendarDbContext _calendarDbContext;

    public EventAppService(CalendarDbContext calendarDbContext)
    {
        _calendarDbContext = calendarDbContext;
    }
}
```

## Database Guidelines

### Entity Framework Patterns
```csharp
// Use async for database operations
public async Task<bool> InviteUsersToEvent(int eventId, List<int> userIds)
{
    var @event = await _calendarDbContext.Events
        .Include(e => e.Owner)
        .FirstOrDefaultAsync(e => e.Id == eventId);
    
    await _calendarDbContext.SaveChangesAsync();
}

// Always use Include for navigation properties
var events = _calendarDbContext.Events
    .Include(e => e.Owner)
    .Include(e => e.EventAttendees)
    .ThenInclude(ea => ea.User)
    .ToList();
```

### Entity Configuration
```csharp
// Use Data Annotations for simple validation
[Required, TextLengthRange(3, 255)]
public string Title { get; set; } = string.Empty;

// Use Fluent API for complex configurations in OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Event>()
        .HasOne(e => e.Owner)
        .WithMany(u => u.OwnedEvents)
        .HasForeignKey(e => e.OwnerId);
}
```

### Migration Best Practices
- Name migrations descriptively (e.g., `AddEventAttendeeTable`)
- Review generated migration code before applying
- Keep migrations small and focused
- Always test migrations on development data

## Frontend Guidelines

### Razor Views
```csharp
// Use ViewModels for data transfer
@model MonthlyCalendarViewModel

// Use Tag Helpers over HTML helpers
<a asp-action="Details" asp-route-id="@event.Id" class="btn btn-primary">
    View Details
</a>

// Use partial views for reusable components
<partial name="_MonthlyCalendar" model="Model" />
```

### Bootstrap Integration
```html
<!-- Use Bootstrap classes consistently -->
<div class="container-fluid">
    <div class="row mb-4">
        <div class="col">
            <h2>Events</h2>
        </div>
        <div class="col-auto">
            <a asp-action="Create" class="btn btn-primary">
                <i class="bi bi-plus-circle"></i> Create New Event
            </a>
        </div>
    </div>
</div>
```

### jQuery Integration
```javascript
// Use jQuery for DOM manipulation and AJAX
$(document).ready(function() {
    $('.event-item').click(function(e) {
        e.stopPropagation();
        var url = $(this).find('.event-link').attr('href');
        if (url) {
            window.location.href = url;
        }
    });
});
```

## Security Guidelines

### Authorization Patterns
```csharp
[Authorize]                           // Require authentication
[Authorize(Roles = "Admin")]         // Require specific role
public class AdminController : Controller
{
    // Check user ownership in controllers
    if (event.OwnerId != userId)
    {
        return Forbid(); // Or return appropriate error
    }
}
```

### Identity Integration
```csharp
// Get current user information
var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (int.TryParse(userIdClaim, out var userId))
{
    // Use userId for authorization
}
```

## Validation Guidelines

### Data Annotations
```csharp
// Use custom validation attributes when needed
[Required, TextLengthRange(3, 255)]
public string Title { get; set; } = string.Empty;

[DateRange] // Custom attribute for date validation
public DateTime EndTime { get; set; }
```

### Server-side Validation
```csharp
// Always validate on server side regardless of client validation
if (ModelState.IsValid)
{
    // Process valid data
}
else
{
    // Return to view with validation errors
    return View(model);
}
```

## CSS Organization

### File Structure
```
wwwroot/css/
├── site.css              # Main stylesheet
├── monthlyView.css        # Calendar-specific styles
└── components/            # Component-specific styles (when splitting)
```

### Class Naming
- Use BEM methodology for complex components
- Keep class names descriptive and consistent
- Use Bootstrap utilities first, custom styles second

```css
.calendar-day {
    /* Base styles */
}

.calendar-day.other-month {
    /* Modifier styles */
    opacity: 0.4;
}

.calendar-day__event {
    /* Element styles */
}
```

## Common Patterns

### Error Handling
```csharp
// Use try-catch for database operations
try
{
    _calendarDbContext.SaveChanges();
    return true;
}
catch (DbUpdateException ex)
{
    // Log error and handle appropriately
    return false;
}
```

### Async/Await Patterns
```csharp
// Always use async for I/O operations
public async Task<Event?> GetByIdAsync(int id)
{
    return await _calendarDbContext.Events
        .Include(e => e.Owner)
        .FirstOrDefaultAsync(e => e.Id == id);
}
```

### DateTime Handling
```csharp
// Use UTC for database storage
public DateTime CreatedAt { get; protected set; }

// Use DateTime.UtcNow for current time
UpdatedAt = DateTime.UtcNow;

// Convert to local time for display
@event.StartTime.ToLocalTime().ToString("MMM dd HH:mm")
```

## Important Notes

- **No Test Framework**: Currently lacks unit/integration tests - consider adding xUnit
- **No Linting**: No .editorconfig or StyleCop configured - consider adding
- **Database**: MySQL 8.0.29 required (run via Docker or local instance)
- **Authentication**: Uses ASP.NET Core Identity with cookie authentication
- **Error Handling**: No centralized error handling - implement global exception filter
- **Logging**: No structured logging - consider adding Serilog

## Project Requirements

Source: `README.md` (Czech, translated). Checked at the week 7 review and defense. Status reflects last known state; verify before claiming done.

### General
- [x] Design document: functional requirements plus entity/class/database design (Word document is acceptable; EA diagrams not required). Created: `docs/CalendarApp_Design_Document.docx` (not visually rendered; LibreOffice not installed).
- [x] ASP.NET Core MVC 9.x or higher (alternatives: Spring MVC, Django MVC, Laravel — not used).
- [x] MVC, multi-layer architecture, object-oriented programming.
- [ ] No SQL code in the web project (stored procedure calls are the only exception). Verify.
- [ ] Layers: Presentation, Application, Infrastructure, Domain (all present; verify boundaries).
- [ ] All functionality created and used through services. Verify controllers have no lower-layer logic.
- [x] Presentation must not use Infrastructure directly, except configuration. `AccountController` now uses `IAccountAppService` (Application). `Program.cs` is configuration (allowed). Verify: `_ViewImports.cshtml` imports `CalendarApp.Infrastructure.Identity`; check views don't use Identity types.
- [x] Database via Code-First with migrations, EF Core.
- [x] At least 5 entities and/or ViewModels/DTOs (EF Core and Identity entities and ErrorViewModel excluded). Entities (4): Event, EventAttendee, Notification, EventOccurrenceException. ViewModels/DTOs (10): UserCreateViewModel, UserListViewModel, EventCreateViewModel, EventEditViewModel, CalendarDayViewModel, AdminEventCreateViewModel, MonthlyCalendarViewModel, RegisterViewModel, LoginViewModel, UserWithRoleDto. Total 14.
- [x] At least one foreign-key relationship.
- [ ] Admin area (`CalendarApp.Web/Areas/Admin`) with management of all DB items, including edit. Verify coverage per entity.
- [ ] Admin may manage user data but must not change password hashes. Deferred; currently unmet.
- [ ] Server-side validation (priority) and client-side validation (preferred).
- [x] At least one custom validation attribute written by us (`TextLengthRange`, `DateRange`; verify both are custom).
- [x] Registration and login via ASP.NET Core Identity.
- [x] At least 2 roles: Admin and User (customer). No Manager role by decision.
- [x] Admin has full rights.
- [ ] Customer sees only own data (own events). Verify all event and notification queries are scoped to the user.
- [x] Responsive front end (Bootstrap 5).

### Operations (from README, not graded)
- Run via Docker Compose (`docker compose up -d`); migrator applies migrations on start.
- Seeding is applied via EF Core `HasData` in `CalendarDbContext` (migration `SeedDataInDbContext`); runs on `docker compose up` via the migrator service.

### References (from README)
- https://www.tutorialsteacher.com/core/aspnet-core-application-project-structure
- https://www.tutorialsteacher.com/core/aspnet-core-wwwroot
- https://www.tutorialsteacher.com/core/aspnet-core-program

## When Making Changes

1. Always follow clean architecture layering
2. Use dependency injection for services
3. Implement both server and client-side validation
4. Keep controllers thin - business logic in services
5. Use async/await for database operations
6. Follow naming conventions consistently
7. Test calendar views with different date scenarios
8. Ensure authorization checks are implemented
9. Keep CSS organized and maintainable
10. Consider responsive design for mobile compatibility