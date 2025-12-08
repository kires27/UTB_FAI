using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.Abstraction
{
    public interface IUserAppService
    {
        Task<(bool Succeeded, string? ErrorMessage)> CreateUserAsync(User user, string password);
        Task<(bool Succeeded, int? UserId, string? Username, string? Role)> ValidateUserAsync(string username, string password);
    }
}