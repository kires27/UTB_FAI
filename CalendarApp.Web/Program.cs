using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CalendarApp.Application.Abstraction;
using CalendarApp.Application.Implementation;
using CalendarApp.Infrastructure.Database;
using CalendarApp.Infrastructure.Database.Seeding;
using CalendarApp.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("MySQL");
ServerVersion serverVersion = new MySqlServerVersion("8.0.29");

builder.Services.AddDbContext<CalendarDbContext>(options => options.UseMySql(connectionString, serverVersion));

// Add ASP.NET Core Identity
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<CalendarDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<IEventAppService, EventAppService>();
builder.Services.AddScoped<INotificationAppService, NotificationAppService>();

var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<CalendarDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();

    // Seed users and roles using Identity (safe - checks for existing)
    var userSeeder = new UsersInit();
    await userSeeder.SeedIdentityUsersAsync(userManager, roleManager);

    // Seed other entities using regular EF (with existence checks)
    if (!await dbContext.Events.AnyAsync())
    {
        var eventSeeder = new EventsInit();
        var events = eventSeeder.GenerateEvents();
        await dbContext.Events.AddRangeAsync(events);
    }

    if (!await dbContext.EventAttendees.AnyAsync())
    {
        var attendeeSeeder = new EventAttendeesInit();
        var attendees = attendeeSeeder.GenerateEventAttendees();
        await dbContext.EventAttendees.AddRangeAsync(attendees);
    }

    if (!await dbContext.Notifications.AnyAsync())
    {
        var notificationSeeder = new NotificationsInit();
        var notifications = notificationSeeder.GenerateNotifications();
        await dbContext.Notifications.AddRangeAsync(notifications);
    }

    await dbContext.SaveChangesAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. you may want to change this for production scenarios, see https://aka.ms/aspnetcore/hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "events",
    pattern: "events/{action=Index}/{id?}",
    defaults: new { controller = "Events" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
