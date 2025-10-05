public class AuthResponseDTO
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public string Role { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}
