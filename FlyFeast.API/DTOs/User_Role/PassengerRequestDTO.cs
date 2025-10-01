namespace FlyFeast.API.DTOs.User_Role
{
    public class PassengerRequestDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string? PassportNumber { get; set; }
        public string? Nationality { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
