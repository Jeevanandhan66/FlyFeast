namespace FlyFeast.API.DTOs.User_Role
{
    public class UserResponseDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();
        public List<PassengerDTO> Passengers { get; set; } = new List<PassengerDTO>();
    }
}
