public class AuthResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string Role { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
