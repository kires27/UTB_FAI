namespace CalendarApp.Application.Abstraction;

public interface IUserService
{
    Task<bool> IsUserInRoleAsync(int userId, string role);
}
