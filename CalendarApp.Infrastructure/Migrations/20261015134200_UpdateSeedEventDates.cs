using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CalendarApp.Infrastructure.Migrations
{
    
    public partial class UpdateSeedEventDates : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -8,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 12, 21, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 21, 12, 30, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -7,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 12, 20, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 20, 18, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -6,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 12, 19, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 19, 19, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -5,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 12, 16, 7, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 16, 6, 30, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 12, 17, 16, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 17, 15, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 12, 20, 15, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 20, 14, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 12, 18, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 18, 11, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2026, 12, 15, 11, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 15, 9, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "AllDay", "Color", "Description", "EndTime", "IsRecurring", "Location", "OwnerId", "RecurrenceEnd", "RecurrenceRule", "ReminderTime", "StartTime", "Status", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { -11, false, "#20c997", "Day hike with friends, bring water and snacks", new DateTime(2026, 11, 21, 17, 0, 0, 0, DateTimeKind.Unspecified), false, "Mountain Trail", -4, null, null, null, new DateTime(2026, 11, 21, 8, 0, 0, 0, DateTimeKind.Unspecified), 0, "Weekend Hiking Trip", new DateTime(2026, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -10, false, "#fd7e14", "Annual car maintenance", new DateTime(2026, 11, 14, 10, 0, 0, 0, DateTimeKind.Unspecified), false, "Auto Service Center", -4, null, null, null, new DateTime(2026, 11, 14, 8, 0, 0, 0, DateTimeKind.Unspecified), 1, "Car Service", new DateTime(2026, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -9, false, "#17a2b8", "Daily team sync", new DateTime(2026, 11, 2, 9, 30, 0, 0, DateTimeKind.Unspecified), true, "Office - Room B", -4, null, "FREQ=WEEKLY;BYDAY=MO,WE,FR", null, new DateTime(2026, 11, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), 0, "Team Standup", new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -6,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 12, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 15, 10, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -5,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 12, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "EventAttendees",
                columns: new[] { "EventId", "UserId", "Role", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { -11, -4, 0, 1, new DateTime(2026, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -11, -3, 1, 0, new DateTime(2026, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -11, -2, 1, 1, new DateTime(2026, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -10, -4, 0, 1, new DateTime(2026, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -9, -4, 0, 1, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "EventId", "IsRead", "Message", "NotifyAt", "Type", "UpdatedAt", "UserId" },
                values: new object[] { -7, -11, false, "Mike Davis invited you to 'Weekend Hiking Trip'", new DateTime(2026, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), -2 });
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -11, -4 });

            migrationBuilder.DeleteData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -11, -3 });

            migrationBuilder.DeleteData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -11, -2 });

            migrationBuilder.DeleteData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -10, -4 });

            migrationBuilder.DeleteData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -9, -4 });

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -11);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -10);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -9);

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -8,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2025, 12, 21, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 21, 12, 30, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -7,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2025, 12, 20, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 20, 18, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -6,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2025, 12, 19, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 19, 19, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -5,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2025, 12, 16, 7, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 16, 6, 30, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2025, 12, 17, 16, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 17, 15, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2025, 12, 20, 15, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 20, 14, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2025, 12, 18, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 18, 11, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2025, 12, 15, 11, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 15, 9, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -6,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 15, 10, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -5,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "NotifyAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
