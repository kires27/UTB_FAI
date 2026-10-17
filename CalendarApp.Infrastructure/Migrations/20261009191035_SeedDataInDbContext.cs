using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CalendarApp.Infrastructure.Migrations
{
    
    public partial class SeedDataInDbContext : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "439ed2ac-e661-4015-9964-e15b29afcdac", "Admin", "ADMIN" },
                    { 2, "fd9ceb09-569e-47d5-b17a-c30ad8117f24", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[,]
                {
                    { -4, 0, "8bb62e2b-1506-42fe-b72e-3dac0cd41c64", "mike.davis@email.com", true, true, null, "MIKE.DAVIS@EMAIL.COM", "MIKEDAVIS", "AQAAAAEAACcQAAAAECGadj9TUQS2u76DLVVo/kkCqwpE1KhvYmrPbxvqQlST8uXxecrCduBUsoGbnC7vTQ==", null, false, "0253c2be-83bf-4bde-a547-0ea099be7c10", false, new DateTime(2025, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "mikedavis" },
                    { -3, 0, "48037745-2d59-43a2-b196-d384b9187764", "sarah.chen@email.com", true, true, null, "SARAH.CHEN@EMAIL.COM", "SARAHCHEN", "AQAAAAEAACcQAAAAECGadj9TUQS2u76DLVVo/kkCqwpE1KhvYmrPbxvqQlST8uXxecrCduBUsoGbnC7vTQ==", null, false, "5904b64c-0089-4b6f-a5cd-cd3fb2a4947b", false, new DateTime(2025, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "sarahchen" },
                    { -2, 0, "309cf6b3-a538-47f8-acd7-740ee59362b6", "alex.johnson@email.com", true, true, null, "ALEX.JOHNSON@EMAIL.COM", "ALEXJOHNSON", "AQAAAAEAACcQAAAAECGadj9TUQS2u76DLVVo/kkCqwpE1KhvYmrPbxvqQlST8uXxecrCduBUsoGbnC7vTQ==", null, false, "d71c3ace-94d7-4576-9d7b-9c5aa98c5a7c", false, new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "alexjohnson" },
                    { -1, 0, "ceec1c53-5dcc-4c96-bd72-51cfd20c3fe6", "admin@calendarapp.com", true, true, null, "ADMIN@CALENDARAPP.COM", "ADMIN", "AQAAAAEAACcQAAAAEKOgH5peMUVS21nMyfE9tbpcPn6U31VL+vpcE3Y/wMAqvGY4JPaPsBV6MFZBx2KPQw==", null, false, "c670b2c6-19b9-4603-9de7-6490e7b1b734", false, new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 2, -4 },
                    { 2, -3 },
                    { 2, -2 },
                    { 1, -1 }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "AllDay", "Color", "Description", "EndTime", "IsRecurring", "Location", "OwnerId", "RecurrenceEnd", "RecurrenceRule", "ReminderTime", "StartTime", "Status", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { -8, false, "#fd7e14", "Monthly catch-up lunch", new DateTime(2025, 12, 21, 14, 0, 0, 0, DateTimeKind.Unspecified), true, "Favorite Restaurant", -3, null, "FREQ=MONTHLY;BYMONTHDAY=15", null, new DateTime(2025, 12, 21, 12, 30, 0, 0, DateTimeKind.Unspecified), 0, "Lunch with Mom", new DateTime(2025, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -7, false, "#e83e8c", "Emma's 30th birthday celebration", new DateTime(2025, 12, 20, 23, 0, 0, 0, DateTimeKind.Unspecified), false, "Emma's House", -2, null, null, null, new DateTime(2025, 12, 20, 18, 0, 0, 0, DateTimeKind.Unspecified), 0, "Birthday Party - Emma", new DateTime(2025, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -6, false, "#ffc107", "Team happy hour after work", new DateTime(2025, 12, 19, 22, 0, 0, 0, DateTimeKind.Unspecified), false, "The Local Pub", -1, null, null, null, new DateTime(2025, 12, 19, 19, 0, 0, 0, DateTimeKind.Unspecified), 0, "Friday Night Drinks", new DateTime(2025, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -5, false, "#dc3545", "Cardio and strength training", new DateTime(2025, 12, 16, 7, 30, 0, 0, DateTimeKind.Unspecified), true, "Gym", -3, null, "FREQ=WEEKLY;BYDAY=TU,TH", null, new DateTime(2025, 12, 16, 6, 30, 0, 0, DateTimeKind.Unspecified), 0, "Morning Workout", new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -4, false, "#6f42c1", "Regular dental checkup", new DateTime(2025, 12, 17, 16, 0, 0, 0, DateTimeKind.Unspecified), false, "Dental Clinic", -2, null, null, null, new DateTime(2025, 12, 17, 15, 0, 0, 0, DateTimeKind.Unspecified), 0, "Dentist Appointment", new DateTime(2025, 11, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -3, false, "#28a745", "Weekly grocery run", new DateTime(2025, 12, 20, 15, 0, 0, 0, DateTimeKind.Unspecified), true, "Supermarket", -2, null, "FREQ=WEEKLY;BYDAY=SA", null, new DateTime(2025, 12, 20, 14, 0, 0, 0, DateTimeKind.Unspecified), 0, "Grocery Shopping", new DateTime(2025, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -2, false, "#007bff", "Discuss project requirements and timeline", new DateTime(2025, 12, 18, 12, 0, 0, 0, DateTimeKind.Unspecified), false, "Virtual - Zoom", -1, null, null, null, new DateTime(2025, 12, 18, 11, 0, 0, 0, DateTimeKind.Unspecified), 0, "Client Call - Project X", new DateTime(2025, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -1, false, "#007bff", "Quarterly business planning and strategy meeting", new DateTime(2025, 12, 15, 11, 0, 0, 0, DateTimeKind.Unspecified), false, "Conference Room A", -1, null, null, null, new DateTime(2025, 12, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), 0, "Q4 Planning Session", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "EventId", "IsRead", "Message", "NotifyAt", "Type", "UpdatedAt", "UserId" },
                values: new object[] { -6, null, false, "Don't forget to bring your gym card for tomorrow's workout!", new DateTime(2025, 12, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2025, 12, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), -3 });

            migrationBuilder.InsertData(
                table: "EventAttendees",
                columns: new[] { "EventId", "UserId", "Role", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { -7, -1, 1, 1, new DateTime(2025, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -6, -3, 1, 3, new DateTime(2025, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -6, -2, 1, 1, new DateTime(2025, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -5, -2, 1, 0, new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -3, -2, 0, 1, new DateTime(2025, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -1, -1, 0, 1, new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "EventId", "IsRead", "Message", "NotifyAt", "Type", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { -5, -2, true, "Event 'Client Call - Project X' has been confirmed", new DateTime(2025, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2025, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), -1 },
                    { -4, -4, false, "Reminder: 'Dentist Appointment' tomorrow at 3:00 PM", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2025, 12, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), -2 },
                    { -3, -7, true, "Sarah Chen invited you to 'Birthday Party - Emma'", new DateTime(2025, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2025, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), -1 },
                    { -2, -6, false, "Alex Johnson invited you to 'Friday Night Drinks'", new DateTime(2025, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2025, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), -3 },
                    { -1, -6, true, "Alex Johnson invited you to 'Friday Night Drinks'", new DateTime(2025, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2025, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), -2 }
                });
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, -4 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, -3 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, -2 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, -1 });

            migrationBuilder.DeleteData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -7, -1 });

            migrationBuilder.DeleteData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -6, -3 });

            migrationBuilder.DeleteData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -6, -2 });

            migrationBuilder.DeleteData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -5, -2 });

            migrationBuilder.DeleteData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -3, -2 });

            migrationBuilder.DeleteData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -1, -1 });

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -8);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1);
        }
    }
}
