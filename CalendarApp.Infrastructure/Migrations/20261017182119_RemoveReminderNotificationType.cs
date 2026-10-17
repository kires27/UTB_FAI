using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarApp.Infrastructure.Migrations
{
    
    public partial class RemoveReminderNotificationType : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                column: "UpdatedAt",
                value: new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                column: "UpdatedAt",
                value: new DateTime(2026, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                column: "UpdatedAt",
                value: new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                column: "UpdatedAt",
                value: new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -6,
                column: "Type",
                value: 2);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: -6,
                column: "Type",
                value: 3);

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "EventId", "IsRead", "Message", "NotifyAt", "Type", "UpdatedAt", "UserId" },
                values: new object[] { -4, -4, false, "Reminder: 'Dentist Appointment' tomorrow at 3:00 PM", new DateTime(2026, 12, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2026, 12, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), -2 });
        }
    }
}
