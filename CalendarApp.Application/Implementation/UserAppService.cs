using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;
using CalendarApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CalendarApp.Application.Implementation
{
    public class UserAppService : IUserAppService
    {
        private readonly CalendarDbContext _calendarDbContext;

        public UserAppService(CalendarDbContext calendarDbContext)
        {
            _calendarDbContext = calendarDbContext;
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> CreateUserAsync(User user, string password)
        {
            try
            {
                // Check if username already exists
                var existingUser = _calendarDbContext.Users.FirstOrDefault(u => u.UserName == user.UserName);
                if (existingUser != null)
                {
                    return (false, "Username already exists");
                }

                // Check if email already exists
                var existingEmail = _calendarDbContext.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingEmail != null)
                {
                    return (false, "Email already exists");
                }

                // Hash password
                user.PasswordHash = HashPassword(password);
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                _calendarDbContext.Users.Add(user);
                await _calendarDbContext.SaveChangesAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Succeeded, int? UserId, string? Username, string? Role)> ValidateUserAsync(string username, string password)
        {
            try
            {
                var user = _calendarDbContext.Users.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                {
                    return (false, null, null, null);
                }

                var hashedPassword = HashPassword(password);
                if (user.PasswordHash != hashedPassword)
                {
                    return (false, null, null, null);
                }

                return (true, user.Id, user.UserName, user.Role);
            }
            catch (Exception ex)
            {
                return (false, null, null, null);
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}