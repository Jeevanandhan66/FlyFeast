namespace FlyFeast.API.DTOs.User_Role
{
    public class UserRequestDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public List<string> RoleNames { get; set; } = new();
    }
}
