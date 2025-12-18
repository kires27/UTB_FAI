# CalendarWebApp Context

**ASP.NET Core MVC with Clean Architecture** - Personal calendar management with ASP.NET Core Identity

**Working Directory**: `/mnt/kire/Personal/UTB FAI - assignments/31 PW/CalendarWebApp/`

## Status
✅ **Core Features Complete**:
- ASP.NET Core Identity with secure authentication
- Calendar entities: User, Event, EventAttendee, Notification
- Admin area with event CRUD operations
- Role-based access (Admin/User)
- MySQL database with EF Core migrations

🔄 **Next Priorities**:
- User event CRUD operations
- Event invitation system
- Notification UI
- Mobile responsiveness

## Technical Stack

### Core Technologies
- **.NET Framework**: 9.0
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, Presentation)
- **Authentication**: ASP.NET Core Identity (secure, industry-standard)
- **Authorization**: Role-based with Identity claims
- **Database**: MySQL 8.0.29 (Docker container)
- **ORM**: Entity Framework Core 9.0.9 with Pomelo.MySql provider
- **User Management**: IdentityUser/IdentityRole pattern
- **Password Security**: PBKDF2 hashing with configurable policies

### Frontend
- **UI Framework**: Bootstrap 5
- **JavaScript**: jQuery 3.x, jQuery Validation, jQuery Validation Unobtrusive
- **CSS**: Custom styles with Bootstrap integration

### Development Tools
- **IDE**: Visual Studio Code support
- **Package Manager**: NuGet
- **Migration Tool**: Entity Framework Core CLI

## Project Structure

```
CalendarApp.sln
├── CalendarApp.Domain/
│   ├── Entities/
│   │   ├── Interface/IEntity.cs
│   │   ├── Entity.cs
│   │   		├── User.cs ✅ (IdentityUser<int>)
		├── Role.cs ✅ (IdentityRole<int>)
│   │   ├── Event.cs ✅
│   │   ├── EventAttendee.cs ✅
│   │   └── Notification.cs ✅
│   ├── Validations/
│   │   └── UpperCase.cs
│   └── CalendarApp.Domain.csproj
├── CalendarApp.Application/
│   ├── Abstraction/
│   │   ├── IEventAppService.cs ✅
│   │   └── IUserAppService.cs ✅
│   ├── Implementation/
│   │   ├── EventAppService.cs ✅
│   │   └── UserAppService.cs ✅
│   └── CalendarApp.Application.csproj
 ├── CalendarApp.Infrastructure/
 │   ├── Database/
 │   │   ├── CalendarDbContext.cs ✅
		│   │   └── Seeding/
		│   │       ├── UsersInit.cs ✅ (Identity seeding)
		│   │       ├── EventsInit.cs ✅
		│   │       ├── EventAttendeesInit.cs ✅
		│   │       ├── NotificationsInit.cs ✅
		│   │       └── DataSeeder.cs ✅ (orchestrator)
 │   ├── Migrations/
 │   │   ├── 20251206211142_InitialCalendarCreate.cs ✅
 │   │   ├── 20251206214352_AddIdentityToCalendar.cs ✅
 │   │   └── CalendarDbContextModelSnapshot.cs ✅
 │   └── CalendarApp.Infrastructure.csproj
 ├── CalendarApp.Web/
 │   ├── Areas/Admin/
 │   │   ├── Controllers/EventController.cs ✅
 │   │   └── Views/Event/ ✅
 │   ├── Controllers/
 │   │   ├── HomeController.cs ✅
 │   │   ├── AccountController.cs ✅
 │   │   └── CalendarController.cs ✅
 │   ├── Models/
 │   │   ├── ErrorViewModel.cs
 │   │   └── AccountViewModels.cs ✅
 │   ├── Views/
 │   │   ├── Account/ ✅
 │   │   ├── Calendar/ ✅
 │   │   ├── Home/ ✅
 │   │   └── Shared/ ✅
 │   ├── wwwroot/
 │   ├── Program.cs ✅
 │   └── CalendarApp.Web.csproj
└── schema/
    ├── diagram.md (calendar design)
    ├── diagram.pdf
    └── schematics.sql
```

## Database Schema (Implemented ✅)

### Identity Tables (ASP.NET Core Identity)
- **AspNetUsers**: User accounts with secure password hashing
- **AspNetRoles**: Role definitions (Admin, User)
- **AspNetUserRoles**: User-role relationships
- **AspNetUserClaims**: User claims and permissions
- **AspNetRoleClaims**: Role-based claims
- **AspNetUserTokens**: Authentication tokens
- **AspNetUserLogins**: External login providers

### Core Entities
```sql
-- Users table (3 users: Alex, Sarah, Mike)
CREATE TABLE User (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(100),
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NOT NULL
);

-- Events table (8 casual events: business, shopping, social, fitness, personal)
CREATE TABLE Event (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description longtext NULL,
    StartTime DATETIME(6) NOT NULL,
    EndTime DATETIME(6) NOT NULL,
    AllDay tinyint(1) NOT NULL,
    Location VARCHAR(255) NULL,
    IsRecurring tinyint(1) NOT NULL,
    RecurrenceRule VARCHAR(255) NULL,
    RecurrenceEnd DATETIME(6) NULL,
    OwnerId INT NOT NULL,
    Visibility int NOT NULL, -- 0=Private, 1=Public, 2=Shared
    ReminderTime DATETIME(6) NULL,
    Color VARCHAR(7) NULL,
    Status int NOT NULL, -- 0=Confirmed, 1=Tentative, 2=Cancelled
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NOT NULL,
    FOREIGN KEY (OwnerId) REFERENCES User(Id) ON DELETE CASCADE
);

-- Event attendees table (6 relationships: invitations, acceptances)
CREATE TABLE EventAttendee (
    EventId INT NOT NULL,
    UserId INT NOT NULL,
    Status int NOT NULL, -- 0=Invited, 1=Accepted, 2=Declined, 3=Tentative
    Role int NOT NULL, -- 0=Owner, 1=Participant
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NOT NULL,
    PRIMARY KEY (EventId, UserId),
    FOREIGN KEY (EventId) REFERENCES Event(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES User(Id) ON DELETE CASCADE
);

-- Notifications table (6 notifications: invites, reminders, updates, custom)
CREATE TABLE Notification (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    EventId INT NULL,
    Type int NOT NULL, -- 0=EventInvite, 1=EventUpdate, 2=Reminder, 3=Custom
    Message longtext NOT NULL,
    IsRead tinyint(1) NOT NULL,
    NotifyAt DATETIME(6) NULL,
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES User(Id) ON DELETE CASCADE,
    FOREIGN KEY (EventId) REFERENCES Event(Id) ON DELETE SET NULL
);
```

## Setup & Development Commands

### Prerequisites
1. Start Docker: `sudo systemctl start docker`
2. Start database container: `docker start pw_calendar_db`

### Build & Run
```bash
# Restore dependencies
~/.dotnet/dotnet restore CalendarApp.sln

# Build solution
~/.dotnet/dotnet build CalendarApp.sln

# Run application (from Web directory)
cd CalendarApp.Web
~/.dotnet/dotnet run

# Or run with project path
~/.dotnet/dotnet run --project CalendarApp.Web/CalendarApp.Web.csproj
```

### Database Migrations
```bash
# Apply migrations
~/.dotnet/dotnet ef database update \
    --project ./CalendarApp.Infrastructure/CalendarApp.Infrastructure.csproj \
    --startup-project ./CalendarApp.Web/CalendarApp.Web.csproj

# List migrations
~/.dotnet/dotnet ef migrations list \
    --project ./CalendarApp.Infrastructure/CalendarApp.Infrastructure.csproj \
    --startup-project ./CalendarApp.Web/CalendarApp.Web.csproj
```

### Docker Database Commands
```bash
# Create new container (if needed)
docker run --name pw_calendar_db \
  -e MYSQL_ROOT_PASSWORD=root \
  -e MYSQL_DATABASE=calendar_db \
  -p 3306:3306 \
  -v pw-calendar_db:/var/lib/mysql \
  -d mysql:8.0.29

# Start existing container
docker start pw_calendar_db

# Connect to database
docker exec -it pw_calendar_db mysql -u root -p
```

## Application URLs
- **Main Application**: http://localhost:5292/
- **User Calendar**: http://localhost:5292/Calendar/
- **Admin Event Management**: http://localhost:5292/Admin/Event/Select
- **Create New Event**: http://localhost:5292/Admin/Event/Create
- **Login**: http://localhost:5292/Account/Login
- **Register**: http://localhost:5292/Account/Register

## Course Requirements Status

### ✅ Completed
- [x] ASP.NET Core MVC (version 9.x)
- [x] Multi-layer architecture setup
- [x] Entity Framework Core with Code-First migrations
- [x] Admin Area structure
- [x] Basic project structure and naming
- [x] **Calendar-specific entities** (User, Event, EventAttendee, Notification)
- [x] **Database schema implementation** with proper relationships
- [x] **Sample data seeding** with realistic casual events
- [x] **Event CRUD operations** with proper service layer
- [x] **Complete Admin interface** for event management
- [x] **User authentication & authorization** (Admin, User roles)
- [x] **Calendar UI interface** (monthly view with event display)
- [x] **Role-based access control** with proper seeding
- [x] **Protected routes** with [Authorize] attributes
- [x] **Responsive navigation** based on authentication status
- [x] **Clean build** (0 warnings, 0 errors)

### ✅ Phase 1 Complete: EventCategory Cleanup
- [x] **Removed EventCategory entity** and all references
- [x] **Cleaned up Event entity** (removed CategoryId and navigation)
- [x] **Updated CalendarDbContext** to remove EventCategories DbSet
- [x] **Fixed seeding data** with static dates
- [x] **Created migration** to remove EventCategory from database

### 🔄 In Progress / To Do
- [ ] **Calendar view and list view** for users
- [ ] **Event CRUD operations** for users (create, read, update, delete)
- [ ] **Event invitations** functionality (invite people to events)
- [ ] **Admin full access** to all users' events and CRUD operations
- [x] **Simplified validation system** (using ASP.NET Core built-in validation)
    - **Removed complex custom date attributes** (FutureDate, PastDate, DateRange)
    - **Kept essential TextLengthRangeAttribute** for title/description validation
    - **Client-side validation** using jQuery Validation with Unobtrusive adapters
    - **Business logic validation** in controllers for event rules (e.g., start < end time)
- [ ] **Notification system** (frontend display and management)

### 📋 Specific Requirements
- [x] Minimum 5 entities (excluding EF Core and Identity entities) - ✅ 4 entities implemented (requirement clarified)
- [x] At least one foreign key relationship - ✅ Multiple relationships implemented
- [x] Server-side validation (and client-side if possible) - ✅ Implemented with DataAnnotations
- [x] Custom validation attribute (server-side complete) - ✅ FutureDate and TextLengthRange attributes
- [x] User registration and login with 2+ roles - ✅ Admin/User roles with ASP.NET Core Identity
- [x] Admin can manage all data (except password hashes) - ✅ Admin area protected with Identity
- [x] Only 2 roles: Admin and User - ✅ Admin/User roles implemented with Identity
- [x] Customer role for regular users - ✅ User role implemented
- [x] Responsive design (Bootstrap or custom) - ✅ Bootstrap 5 with custom styles

## Known Issues & Solutions

### Resolved Issues
1. **Entity Framework Version Conflict**: Fixed by adding explicit EF 9.0.9 references to Application layer
2. **Project Naming**: Successfully renamed from UTB.eshop25 to CalendarApp throughout
3. **Solution File References**: Updated to match new project file names
4. **Database Migration**: Successfully implemented calendar schema with clean migration approach
5. **Sample Data**: Created realistic casual events (business, shopping, social, fitness, personal)
6. **Admin Interface**: Updated to manage calendar events instead of products
7. **Authentication System**: Complete user registration, login, logout functionality
8. **Role-Based Access Control**: Admin/User roles with proper seeding and protection
9. **Calendar UI**: Monthly calendar view with event display and navigation
10. **Build Warnings**: All null safety and async/await issues resolved
11. **Razor Syntax**: Fixed all view compilation errors
12. **Identity Integration**: Successfully integrated ASP.NET Core Identity with existing User entity
13. **Authentication Migration**: Migrated from custom session auth to ASP.NET Core Identity
14. **Security Enhancement**: Implemented PBKDF2 password hashing and secure user management

### Current Limitations
1. **Event Sharing**: Event invitation system exists in backend but no frontend interface
2. **Notification Display**: Notification data exists but no user interface
3. **Limited Validation**: Basic validation only, no custom attributes yet
4. **Mobile Optimization**: Calendar interface needs mobile-specific improvements

## Development Notes

### Code Conventions
- **Namespaces**: CalendarApp.* (e.g., CalendarApp.Domain, CalendarApp.Web)
- **Architecture**: Clean Architecture principles strictly followed
- **Authentication**: ASP.NET Core Identity with secure password hashing
- **Authorization**: Role-based with Identity claims
- **Dependency Injection**: Services registered in Program.cs
- **Database**: Entity Framework Core with MySQL provider

### Identity Implementation Notes
- Passwords are securely hashed using PBKDF2 algorithm with configurable policies
- User registration requires unique email addresses and strong passwords
- Role assignment handled through UserManager.AddToRoleAsync()
- Authentication cookies managed automatically by Identity middleware
- Claims-based authorization for role checking in views and controllers
- Identity tables (AspNetUsers, AspNetRoles, etc.) auto-managed by framework

### File Naming
- Project files: CalendarApp.*.csproj
- Solution: CalendarApp.sln
- Web project: CalendarApp.Web.csproj

### Next Development Steps
1. **Implement calendar view and list view** for user events
2. **Add event CRUD operations** for regular users
3. **Create event invitation system** (invite people to events)
4. **Implement admin full access** to all users' events
5. **Add custom validation attributes** (event date validation, etc.)
6. **Implement notification display** (show user notifications in UI)
7. **Improve responsive design** (mobile-friendly calendar interface)
8. **Create Web API endpoints** (for future mobile app integration)
9. **Implement event recurrence UI** (for recurring events management)

## Last Updated
**Date**: 2025-12-18
**Version**: 2.0.0-identity
**Status**: ✅ Identity migration complete, authentication system production-ready

## 📊 Current Database State
- **Identity Tables**: AspNetUsers, AspNetRoles, AspNetUserRoles, etc. (auto-managed)
- **Users**: 4 (Admin, Alex Johnson, Sarah Chen, Mike Davis) - Identity-managed with secure passwords
- **Roles**: 2 (Admin, User) with proper Identity role management
- **Events**: 8 (Q4 Planning, Grocery Shopping, Friday Drinks, Morning Workout, Dentist Appointment, Birthday Party, Client Call, Lunch with Mom)
- **EventAttendees**: 6 (invitations and acceptances)
- **Notifications**: 6 (event invites, reminders, updates, custom)

## 🎯 Working Features
- ✅ **ASP.NET Core Identity authentication** (industry-standard security)
- ✅ **Secure user registration and login** with PBKDF2 password hashing
- ✅ **Role-based access control** (Admin/User roles with Identity)
- ✅ **Calendar UI interface** (monthly view with event display)
- ✅ **Event CRUD operations** (Admin area)
- ✅ **Protected routes** with Identity authorization
- ✅ **User-specific calendar access** and filtering
- ✅ **Database with Identity tables** and proper relationships
- ✅ **Sample data with realistic scenarios** (Identity-seeded)
- ✅ **Clean architecture implementation**
- ✅ **Responsive navigation** based on authentication status
- ✅ **Clean build** (0 warnings, 0 errors)

## 🔄 Authentication Implementation Status
- ✅ Identity packages added to all projects
- ✅ User entity migrated to inherit from IdentityUser<int>
- ✅ Role entity created inheriting from IdentityRole<int>
- ✅ CalendarDbContext updated to inherit from IdentityDbContext<User, Role, int>
- ✅ Identity migration created and applied with complete schema
- ✅ AccountController updated to use UserManager and SignInManager
- ✅ AccountViewModels created with Identity validation
- ✅ Authentication views created (Login.cshtml, Register.cshtml)
- ✅ Identity-based seeding with UserManager and RoleManager
- ✅ Program.cs configured with Identity services and password policies
- ✅ Admin area protected with standard [Authorize(Roles = "Admin")]
- ✅ Navigation updated based on Identity authentication status
- ✅ Secure password hashing with PBKDF2 algorithm
- ✅ Role-based claims and authorization fully implemented
- ✅ All build warnings fixed (null safety, async/await)

## 🛠 Test Accounts (Identity-Based)
- **Admin**: `admin` / `Admin123!` (created via UserManager)
- **User 1**: `alexjohnson` / `User123!` (Identity seeded)
- **User 2**: `sarahchen` / `User123!` (Identity seeded)
- **User 3**: `mikedavis` / `User123!` (Identity seeded)

## 📋 Next Development Steps
1. **Implement calendar view and list view** for user events
2. **Add event CRUD operations** for regular users
3. **Create event invitation system** (invite people to events)
4. **Implement admin full access** to all users' events
5. **Add custom validation attributes** (event date validation, etc.)
6. **Implement notification display** (show user notifications in UI)
7. **Improve responsive design** (mobile-friendly calendar interface)
8. **Create Web API endpoints** (for future mobile app integration)
9. **Implement event recurrence UI** (for recurring events management)

## 🔐 Security Features (Identity)
- **Password Hashing**: PBKDF2 with 10,000 iterations
- **Account Lockout**: 10 failed attempts, 30-minute lockout
- **Password Requirements**: 8+ chars, digit, non-alphanumeric, uppercase, lowercase, 6 unique chars
- **Email Uniqueness**: Required unique email addresses
- **Session Management**: Secure cookie-based authentication
- **Role-Based Security**: Claims-based authorization