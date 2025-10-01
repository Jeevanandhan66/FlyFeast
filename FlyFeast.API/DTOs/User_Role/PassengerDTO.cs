namespace FlyFeast.API.DTOs.User_Role
{
    public class PassengerDTO
    {
        public int PassengerId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? PassportNumber { get; set; }
        public string? Nationality { get; set; }
    }
}
