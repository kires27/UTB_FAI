namespace CalendarApp.Application.Abstraction
{
    public interface IUserAppService
    {
        Task<IList<UserWithRoleDto>> GetAllUsersAsync();
        Task<bool> CreateUserAsync(string username, string email, string password, string role);
        Task<bool> DeleteUserAsync(int userId);
    }

    public class UserWithRoleDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}