using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarApp.Infrastructure.Migrations
{
    
    public partial class UpdateSeedTimestamps : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -7, -1 },
                column: "UpdatedAt",
                value: new DateTime(2026, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -6, -3 },
                column: "UpdatedAt",
                value: new DateTime(2026, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -6, -2 },
                column: "UpdatedAt",
                value: new DateTime(2026, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -5, -2 },
                column: "UpdatedAt",
                value: new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -3, -2 },
                column: "UpdatedAt",
                value: new DateTime(2026, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -1, -1 },
                column: "UpdatedAt",
                value: new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -8,
                column: "UpdatedAt",
                value: new DateTime(2026, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -7,
                column: "UpdatedAt",
                value: new DateTime(2026, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -6,
                column: "UpdatedAt",
                value: new DateTime(2026, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -5,
                column: "UpdatedAt",
                value: new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -4,
                column: "UpdatedAt",
                value: new DateTime(2026, 11, 27, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -3,
                column: "UpdatedAt",
                value: new DateTime(2026, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -2,
                column: "UpdatedAt",
                value: new DateTime(2026, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -1,
                column: "UpdatedAt",
                value: new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -7, -1 },
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -6, -3 },
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -6, -2 },
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -5, -2 },
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -3, -2 },
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "EventAttendees",
                keyColumns: new[] { "EventId", "UserId" },
                keyValues: new object[] { -1, -1 },
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -8,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -7,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -6,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -5,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -4,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 27, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -3,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -2,
                column: "UpdatedAt",
                value: new DateTime(2025, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: -1,
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
